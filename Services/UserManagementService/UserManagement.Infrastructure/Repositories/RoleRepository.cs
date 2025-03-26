using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Data;
using UserManagement.Infrastructure.Repositories.IRepositories;

namespace UserManagement.Infrastructure.Repositories;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(UserManagementDbContext context) : base(context) { }

    public async Task<Role?> GetByNameAsync(string roleName) => await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
}
