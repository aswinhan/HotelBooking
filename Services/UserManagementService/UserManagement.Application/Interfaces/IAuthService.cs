using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.Interfaces;

public interface IAuthService
{
    Task<string> LoginWithEmailAsync(string email, string password);
    Task<string> LoginWithPhoneAsync(string phoneNumber, string otp);
    Task<string> LoginWithGoogleAsync(string googleToken);
}
