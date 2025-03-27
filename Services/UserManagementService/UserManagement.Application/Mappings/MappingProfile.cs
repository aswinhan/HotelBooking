using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UserManagement.Application.DTOs.UserDTO;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserProfile, UserProfileDto>();
        CreateMap<UserProfileUpdateDto, UserProfile>();
    }
}
