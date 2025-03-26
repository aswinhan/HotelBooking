using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Data;
using UserManagement.Infrastructure.Repositories.IRepositories;

namespace UserManagement.Infrastructure.Repositories;

public class RoleRepository(UserManagementDbContext context) : GenericRepository<Role>(context), IRoleRepository
{
    public async Task<Role?> GetByNameAsync(string roleName) => await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
}
