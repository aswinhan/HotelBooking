using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Interfaces;
using UserManagement.Application.DTOs;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using static UserManagement.Application.DTOs.AuthDTO;
using UserManagement.API.Extensions;

namespace UserManagement.API.Endpoints;

public static class AuthEndpoints
{
    public static void ConfigureAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/google-login", GoogleLogin)
            .WithName("GoogleLogin")
            .Produces(200)
            .Produces(401);

        app.MapPost("/api/auth/register", Register)
            .WithName("Register")
            .Produces(200)
            .Produces(400);

        app.MapPost("/api/auth/login", Login)
            .WithName("Login")
            .Produces(200)
            .Produces(400);

        app.MapPost("/api/auth/refresh", Refresh)
            .WithName("Refresh")
            .Produces(200)
            .Produces(400);

        app.MapPost("/api/auth/logout", Logout)
            .WithName("Logout")
            .Produces(200)
            .Produces(400);

        // Request Host Verification (Only Guests)
        app.MapPost("/api/host/request-verification", async ([FromBody] HostVerificationRequest request, IAuthService authService, ClaimsPrincipal user) =>
        {
            if (!user.IsInRole("Guest"))
                return Results.Forbid();

            await authService.RequestHostVerification(user.GetUserId(), request);
            return Results.Ok("Host verification requested");
        }).RequireAuthorization();

        // Approve Host (Only Admins)
        app.MapPost("/api/admin/approve-host", async ([FromBody] Guid userId, IAuthService authService, ClaimsPrincipal user) =>
        {
            if (!user.IsInRole("Admin"))
                return Results.Forbid();

            await authService.ApproveHost(userId);
            return Results.Ok("Host approved successfully");
        }).RequireAuthorization();

        // 1️⃣ Request Password Reset OTP
        app.MapPost("/api/auth/forgot-password", async ([FromBody] ForgotPasswordRequest request, IAuthService authService) =>
        {
            await authService.SendPasswordResetOtpAsync(request);
            return Results.Ok("OTP sent to your email");
        });

        // 2️⃣ Reset Password
        app.MapPost("/api/auth/reset-password", async ([FromBody] ResetPasswordRequest request, IAuthService authService) =>
        {
            await authService.ResetPasswordAsync(request);
            return Results.Ok("Password reset successful");
        });
    }

    private async static Task<IResult> GoogleLogin([FromBody] string googleToken, IAuthService authService)
    {
        var token = await authService.LoginWithGoogleAsync(googleToken);
        return token != null ? Results.Ok(new { Token = token }) : Results.Unauthorized();
    }

    private async static Task<IResult> Register([FromBody] AuthDTO.RegisterRequest request, IAuthService authService)
    {
        var token = await authService.RegisterAsync(request.Email, request.Password, request.FirstName, request.LastName);
        return Results.Ok(new { Token = token });
    }

    private async static Task<IResult> Login([FromBody] AuthDTO.LoginRequest request, IAuthService authService)
    {
        var response = await authService.LoginAsync(request.Email, request.Password);
        return Results.Ok(response);
    }

    private async static Task<IResult> Refresh([FromBody] AuthDTO.RefreshTokenRequest request, IAuthService authService)
    {
        var response = await authService.RefreshTokenAsync(request.RefreshToken);
        return Results.Ok(response);
    }

    private async static Task<IResult> Logout([FromBody] AuthDTO.LogoutRequest request, IAuthService authService)
    {
        await authService.LogoutAsync(request.UserId);
        return Results.Ok("Logged out successfully");
    }
}
