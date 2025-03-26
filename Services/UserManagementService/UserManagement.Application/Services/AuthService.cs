using UserManagement.Application.Interfaces;
using UserManagement.Infrastructure.Security;
using UserManagement.Infrastructure.Repositories.IRepositories;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Services;

public class AuthService : IAuthService


// Rest of the code remains unchanged
{
    private readonly IUserRepository _userRepository;
    private readonly TokenService _tokenService;
    private readonly IGoogleAuthService _googleAuthService;

    public AuthService(IUserRepository userRepository, TokenService tokenService, IGoogleAuthService googleAuthService)
    {
        _userRepository = userRepository;
        _googleAuthService = googleAuthService;
        _tokenService = tokenService;
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
        var matchedUser = user.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
        if (matchedUser == null) throw new UnauthorizedAccessException("User not found");

        return _tokenService.GenerateToken(matchedUser);
    }

    public async Task<string> LoginWithGoogleAsync(string googleToken)
    {
        var googleUser = await _googleAuthService.ValidateGoogleTokenAsync(googleToken);
        if (googleUser == null)
            throw new UnauthorizedAccessException("Invalid Google token");

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

    private bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
