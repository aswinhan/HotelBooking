using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.DTOs
{
    public class UserDTO
    {
        public record UserProfileDto(Guid Id, string Name, string Email, string PhoneNumber, string Address, string Role);
        public record class UserProfileUpdateDto
        {
            public string Name { get; init; } = string.Empty;
            public string PhoneNumber { get; init; } = string.Empty;
            public string Address { get; init; } = string.Empty;
        }
    }
}
