using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Repositories.IRepositories;

public interface IHostPropertyRepository : IGenericRepository<HostProperty>
{
    Task<IEnumerable<HostProperty>> GetByHostIdAsync(Guid hostId);
}
