using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Interfaces;
using UserManagement.Application.DTOs;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using static UserManagement.Application.DTOs.AuthDTO;
using UserManagement.API.Extensions;
using static UserManagement.Application.DTOs.UserDTO;
using FluentValidation;

namespace UserManagement.API.Endpoints;

public static class UserEndpoints
{
    public static void ConfigureUserEndpoints(this WebApplication app)
    {
        app.MapGet("/api/user/profile", async (IUserProfileService userProfileService, ClaimsPrincipal user) =>
        {
            var userId = user.GetUserId();
            var profile = await userProfileService.GetProfileAsync(userId);

            return profile is null ? Results.NotFound("Profile not found") : Results.Ok(profile);
        }).RequireAuthorization();

        app.MapPut("/api/user/profile", async ([FromBody] UserProfileUpdateDto updateDto,
    IValidator<UserProfileUpdateDto> validator, IUserProfileService userProfileService,
    ClaimsPrincipal user) =>
        {
            var validationResult = await validator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var userId = user.GetUserId();
            await userProfileService.UpdateProfileAsync(userId, updateDto);

            return Results.Ok("Profile updated successfully");
        }).RequireAuthorization();
    }
}
