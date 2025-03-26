using Microsoft.AspNetCore.Mvc;
using System.Net;
using UserManagement.Application.Interfaces;
using UserManagement.Application.Services;

namespace UserManagement.API.Endpoints;

public static class AuthEndpoints
{
    public static void ConfigureAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/google-login", GoogleLogin)
            .WithName("GoogleLogin")
            .Produces(200)
            .Produces(401);
    }

    private async static Task<IResult> GoogleLogin([FromBody] string googleToken, IAuthService authService)
    {
        var token = await authService.LoginWithGoogleAsync(googleToken);
        return token != null ? Results.Ok(new { Token = token }) : Results.Unauthorized();
    }
}
