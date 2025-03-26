using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.Interfaces;

public interface IRefreshTokenService
{
    string GenerateRefreshToken(Guid userId);
    Task<Guid> ValidateRefreshToken(string refreshToken);
    Task RevokeRefreshToken(Guid userId);
}
