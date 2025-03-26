using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Data;
using UserManagement.Infrastructure.Repositories.IRepositories;

namespace UserManagement.Infrastructure.Repositories;

public class HostPropertyRepository(UserManagementDbContext context) : GenericRepository<HostProperty>(context), IHostPropertyRepository
{
    public async Task<IEnumerable<HostProperty>> GetByHostIdAsync(Guid hostId) => await _context.HostProperties.Where(hp => hp.HostId == hostId).ToListAsync();
}
