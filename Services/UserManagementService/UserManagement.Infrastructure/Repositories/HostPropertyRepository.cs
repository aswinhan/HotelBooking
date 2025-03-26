using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Data;
using UserManagement.Infrastructure.Repositories.IRepositories;

namespace UserManagement.Infrastructure.Repositories;

public class HostPropertyRepository : GenericRepository<HostProperty>, IHostPropertyRepository
{
    public HostPropertyRepository(UserManagementDbContext context) : base(context) { }

    public async Task<IEnumerable<HostProperty>> GetByHostIdAsync(Guid hostId) => await _context.HostProperties.Where(hp => hp.HostId == hostId).ToListAsync();
}
