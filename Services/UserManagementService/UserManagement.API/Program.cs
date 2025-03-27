using FluentValidation;
using FluentValidation.AspNetCore;
using UserManagement.API.Endpoints;
using UserManagement.Application.Interfaces;
using UserManagement.Application.Services;
using UserManagement.Application.Validators;
using UserManagement.Infrastructure.DependencyInjection;
using static UserManagement.Application.DTOs.UserDTO;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddInfrastructure(builder.Configuration);
// Register Service in Application Layer
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddSingleton<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddScoped<IValidator<UserProfileUpdateDto>, UserProfileUpdateDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
// Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.ConfigureAuthEndpoints();
app.ConfigureUserEndpoints();

app.Run();
