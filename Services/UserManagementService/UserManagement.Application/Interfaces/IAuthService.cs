using static UserManagement.Application.DTOs.AuthDTO;
using static UserManagement.Application.Services.AuthService;

namespace UserManagement.Application.Interfaces;

public interface IAuthService
{
    Task<string> LoginWithEmailAsync(string email, string password);
    Task<string> LoginWithPhoneAsync(string phoneNumber, string otp);
    Task<string> LoginWithGoogleAsync(string googleToken);
    Task<string> RegisterAsync(string email, string password, string firstName, string lastName);
    Task<AuthResponse> LoginAsync(string email, string password);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(Guid userId);
    Task RequestHostVerification(Guid userId, HostVerificationRequest request);
    Task ApproveHost(Guid userId);
    Task SendPasswordResetOtpAsync(ForgotPasswordRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
}
