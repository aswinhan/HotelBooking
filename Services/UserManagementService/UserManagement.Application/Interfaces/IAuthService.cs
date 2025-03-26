using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
