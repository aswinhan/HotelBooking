using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UserManagement.Application.DTOs.UserDTO;
using UserManagement.Application.Interfaces;
using UserManagement.Infrastructure.Repositories.IRepositories;
using AutoMapper;

namespace UserManagement.Application.Services;

public class UserProfileService(
    IUserProfileRepository userProfileRepository,
    IMapper mapper) : IUserProfileService
{
    private readonly IUserProfileRepository _userProfileRepository = userProfileRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<UserProfileDto?> GetProfileAsync(Guid userId)
    {
        var profile = await _userProfileRepository.GetByUserIdAsync(userId);
        return profile is null ? null : _mapper.Map<UserProfileDto>(profile);
    }

    public async Task UpdateProfileAsync(Guid userId, UserProfileUpdateDto updateDto)
    {
        var profile = await _userProfileRepository.GetByUserIdAsync(userId) ?? throw new KeyNotFoundException("Profile not found");
        _mapper.Map(updateDto, profile);
        await _userProfileRepository.UpdateAsync(profile);
    }
}
