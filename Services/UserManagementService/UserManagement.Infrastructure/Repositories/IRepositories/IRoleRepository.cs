using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Repositories.IRepositories;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetByNameAsync(string roleName);
}
