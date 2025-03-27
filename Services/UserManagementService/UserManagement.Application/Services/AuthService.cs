using UserManagement.Application.Interfaces;
using UserManagement.Infrastructure.Security;
using UserManagement.Infrastructure.Repositories.IRepositories;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Repositories;
using static UserManagement.Application.DTOs.AuthDTO;
using System.Collections.Concurrent;

namespace UserManagement.Application.Services;

public class AuthService(IUserRepository userRepository, TokenService tokenService, IGoogleAuthService googleAuthService, IRefreshTokenService refreshTokenService, IHostVerificationRepository hostVerificationRepository, IRoleRepository roleRepository, IEmailService emailService) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly TokenService _tokenService = tokenService;
    private readonly IGoogleAuthService _googleAuthService = googleAuthService;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
    private readonly IHostVerificationRepository _hostVerificationRepository = hostVerificationRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IEmailService _emailService = emailService;
    // In-memory store for OTPs (use Redis/DB in production)
    private static readonly ConcurrentDictionary<string, string> _otpStore = new();

    public async Task<string> RegisterAsync(string email, string password, string firstName, string lastName)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
            throw new InvalidOperationException("Email is already registered.");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = hashedPassword,
            FirstName = firstName,
            LastName = lastName,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow,
            UserRoles = [] // Initialize UserRoles to avoid null reference
        };
        // Assign default "Guest" role
        var guestRole = await _roleRepository.GetByNameAsync("Guest");
        newUser.UserRoles.Add(new UserRole { User = newUser, Role = guestRole });

        await _userRepository.AddAsync(newUser);
        return _tokenService.GenerateToken(newUser);
    }

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password");

        var accessToken = _tokenService.GenerateToken(user);
        var refreshToken = _refreshTokenService.GenerateRefreshToken(user.Id);
        return new AuthResponse { AccessToken = accessToken, RefreshToken = refreshToken };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var userId = await _refreshTokenService.ValidateRefreshToken(refreshToken);
        if (userId == Guid.Empty)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var user = await _userRepository.GetByIdAsync(userId) ?? throw new UnauthorizedAccessException("User not found");
        var newAccessToken = _tokenService.GenerateToken(user);
        var newRefreshToken = _refreshTokenService.GenerateRefreshToken(user.Id);
        return new AuthResponse { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
    }

    public async Task<string> LoginWithEmailAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !VerifyPassword(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password");

        return _tokenService.GenerateToken(user);
    }

    public async Task<string> LoginWithPhoneAsync(string phoneNumber, string otp)
    {
        // Simulate OTP verification (Implement actual OTP service later)
        if (otp != "123456") throw new UnauthorizedAccessException("Invalid OTP");
        var user = await _userRepository.GetAllAsync();
        var matchedUser = user.FirstOrDefault(u => u.PhoneNumber == phoneNumber) ?? throw new UnauthorizedAccessException("User not found");
        return _tokenService.GenerateToken(matchedUser);
    }

    public async Task<string> LoginWithGoogleAsync(string googleToken)
    {
        var googleUser = await _googleAuthService.ValidateGoogleTokenAsync(googleToken) ?? throw new UnauthorizedAccessException("Invalid Google token");
        if (string.IsNullOrEmpty(googleUser.Email))
            throw new UnauthorizedAccessException("Google user email is null or empty");

        var user = await _userRepository.GetByEmailAsync(googleUser.Email);
        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = googleUser.Email,
                FirstName = googleUser.Name?.Split(" ")[0] ?? "",
                LastName = googleUser.Name?.Split(" ")[1] ?? "",
                IsVerified = true,
                CreatedAt = DateTime.UtcNow,
                UserRoles = [] // Initialize UserRoles to avoid null reference
            };
            await _userRepository.AddAsync(user);
        }

        return _tokenService.GenerateToken(user);
    }

    public async Task LogoutAsync(Guid userId)
    {
        await _refreshTokenService.RevokeRefreshToken(userId);
    }

    public async Task RequestHostVerification(Guid userId, HostVerificationRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new ArgumentException("User not found");
        var existingVerification = await _hostVerificationRepository.GetByUserIdAsync(userId);
        if (existingVerification != null)
            throw new InvalidOperationException("Host verification already requested");

        var verification = new HostVerification
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            GovernmentId = request.GovernmentId,
            BusinessLicense = request.BusinessLicense,
            IsVerified = false
        };

        await _hostVerificationRepository.AddAsync(verification);
    }

    // Admin approves the host request (One-Time Approval)
    public async Task ApproveHost(Guid userId)
    {
        var verification = await _hostVerificationRepository.GetByUserIdAsync(userId);
        if (verification == null || verification.IsVerified)
            throw new InvalidOperationException("Host verification not found or already approved");

        verification.IsVerified = true;

        var user = await _userRepository.GetByIdAsync(userId);
        var hostRole = await _roleRepository.GetByNameAsync("Host");
        user.UserRoles ??= []; // Ensure UserRoles is initialized
        user.UserRoles.Add(new UserRole { User = user, Role = hostRole });
    }

    // 1️⃣ Generate OTP & Send Email
    public async Task SendPasswordResetOtpAsync(ForgotPasswordRequest request)
    {
        var otp = new Random().Next(100000, 999999).ToString();
        _otpStore[request.Email] = otp;

        await _emailService.SendEmailAsync(request.Email, "Password Reset OTP", $"Your OTP is: {otp}");
    }

    // 2️⃣ Verify OTP & Reset Password
    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        if (!_otpStore.TryGetValue(request.Email, out var storedOtp) || storedOtp != request.Otp)
            throw new UnauthorizedAccessException("Invalid OTP");

        var user = await _userRepository.GetByEmailAsync(request.Email) ?? throw new ArgumentException("User not found");
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        _userRepository.Update(user);

        _otpStore.TryRemove(request.Email, out _);
    }

    private static bool VerifyPassword(string password, string hashedPassword) => BCrypt.Net.BCrypt.Verify(password, hashedPassword);

    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
