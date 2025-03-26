using UserManagement.Application.Interfaces;
using UserManagement.Infrastructure.Security;
using UserManagement.Infrastructure.Repositories.IRepositories;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Services;

public class AuthService(IUserRepository userRepository, TokenService tokenService, IGoogleAuthService googleAuthService, IRefreshTokenService refreshTokenService) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly TokenService _tokenService = tokenService;
    private readonly IGoogleAuthService _googleAuthService = googleAuthService;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;

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
            CreatedAt = DateTime.UtcNow
        };

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
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);
        }

        return _tokenService.GenerateToken(user);
    }

    public async Task LogoutAsync(Guid userId)
    {
        await _refreshTokenService.RevokeRefreshToken(userId);
    }

    private static bool VerifyPassword(string password, string hashedPassword) => BCrypt.Net.BCrypt.Verify(password, hashedPassword);

    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
