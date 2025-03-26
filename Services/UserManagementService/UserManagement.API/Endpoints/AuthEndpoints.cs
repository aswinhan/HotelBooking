//using Google.Apis.Auth.OAuth2.Requests;
//using Microsoft.AspNetCore.Identity.Data;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Win32;
//using System.Net;
//using UserManagement.Application.Interfaces;
//using UserManagement.Application.Services;
//using UserManagement.Application.DTOs.AuthDTO

//using UserManagement.Application.DTOs;

//namespace UserManagement.API.Endpoints;

//public static class AuthEndpoints
//{
//    public static void ConfigureAuthEndpoints(this WebApplication app)
//    {
//        app.MapPost("/api/auth/google-login", GoogleLogin)
//            .WithName("GoogleLogin")
//            .Produces(200)
//        .Produces(401);

//        app.MapPost("/api/auth/register", Register)
//            .WithName("Register")
//            .Produces(200)
//            .Produces(400);

//        app.MapPost("/api/auth/login", Login)
//            .WithName("Login")
//            .Produces(200)
//            .Produces(400);

//        app.MapPost("/api/auth/refresh", Refresh)
//            .WithName("Refresh")
//            .Produces(200)
//            .Produces(400);

//        app.MapPost("/api/auth/logout", Logout)
//            .WithName("Logout")
//            .Produces(200)
//            .Produces(400);

//        app.MapPost("/api/auth/logout", async ([FromBody] LogoutRequest request, IAuthService authService) =>
//        {
//            await authService.LogoutAsync(request.UserId);
//            return Results.Ok("Logged out successfully");
//        });
//    }

//    private async static Task<IResult> GoogleLogin([FromBody] string googleToken, IAuthService authService)
//    {
//        var token = await authService.LoginWithGoogleAsync(googleToken);
//        return token != null ? Results.Ok(new { Token = token }) : Results.Unauthorized();
//    }
//    private async static Task<IResult> Register([FromBody] RegisterRequest request, IAuthService authService)
//    {
//        var token = await authService.RegisterAsync(request.Email, request.Password, request.FirstName, request.LastName);
//        return Results.Ok(new { Token = token });
//    }
//    private async static Task<IResult> Login([FromBody] LoginRequest request, IAuthService authService)
//    {
//        var response = await authService.LoginAsync(request.Email, request.Password);
//        return Results.Ok(response);
//    }
//    private async static Task<IResult> Refresh([FromBody] RefreshTokenRequest request, IAuthService authService)
//    {
//        var response = await authService.RefreshTokenAsync(request.RefreshToken);
//        return Results.Ok(response);
//    }
//    private async static Task<IResult> Logout([FromBody] LogoutRequest request, IAuthService authService)
//    {
//        await authService.LogoutAsync(request.UserId);
//        return Results.Ok("Logged out successfully");
//    }

//}
using Google.Apis.Auth.OAuth2.Requests;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Net;
using UserManagement.Application.Interfaces;
using UserManagement.Application.Services;
using UserManagement.Application.DTOs;

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
