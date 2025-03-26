using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Interfaces;

namespace UserManagement.Application.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private static readonly ConcurrentDictionary<Guid, string> RefreshTokens = new();

    public string GenerateRefreshToken(Guid userId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        RefreshTokens[userId] = token;
        return token;
    }

    public async Task<Guid> ValidateRefreshToken(string refreshToken)
    {
        var userId = RefreshTokens.FirstOrDefault(rt => rt.Value == refreshToken).Key;
        return await Task.FromResult(userId);
    }

    public async Task RevokeRefreshToken(Guid userId)
    {
        RefreshTokens.TryRemove(userId, out _);
        await Task.CompletedTask;
    }
}
