using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UserManagement.Application.DTOs.UserDTO;

namespace UserManagement.Application.Interfaces;

public interface IUserProfileService
{
    Task<UserProfileDto?> GetProfileAsync(Guid userId);
    Task UpdateProfileAsync(Guid userId, UserProfileUpdateDto updateDto);
}
