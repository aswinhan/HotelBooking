using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.DTOs;


public class AuthDTO
{
    public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
    public record LoginRequest(string Email, string Password);
    public record RefreshTokenRequest(string RefreshToken);
    public record LogoutRequest(Guid UserId);
    public record HostVerificationRequest(string GovernmentId, string BusinessLicense, string Address, string GeoLocation);
    public record ForgotPasswordRequest(string Email);
    public record ResetPasswordRequest(string Email, string Otp, string NewPassword);
}
