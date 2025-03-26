using Microsoft.Extensions.Configuration;
using UserManagement.Application.Interfaces;
using Google.Apis.Auth;

namespace UserManagement.Application.Services;

public class GoogleAuthService(IConfiguration configuration) : IGoogleAuthService
{
    private readonly string _clientId = configuration["GoogleAuthSettings:ClientId"] ?? throw new ArgumentNullException("Google Client ID is missing");

    public async Task<GoogleUserInfo?> ValidateGoogleTokenAsync(string googleToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [_clientId]
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken, settings);
        if (payload == null) return null;

        return new GoogleUserInfo
        {
            Id = payload.Subject,
            Email = payload.Email,
            Name = payload.Name,
            Picture = payload.Picture
        };
    }
}
