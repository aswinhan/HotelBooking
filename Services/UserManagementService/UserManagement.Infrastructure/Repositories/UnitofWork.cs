using UserManagement.Infrastructure.Data;
using UserManagement.Infrastructure.Repositories.IRepositories;

namespace UserManagement.Infrastructure.Repositories;

public class UnitOfWork(UserManagementDbContext context, IUserRepository userRepository, IRoleRepository roleRepository, IHostPropertyRepository hostPropertyRepository) : IUnitOfWork
{
    private readonly UserManagementDbContext _context = context;

    public IUserRepository Users { get; } = userRepository;
    public IRoleRepository Roles { get; } = roleRepository;
    public IHostPropertyRepository HostProperties { get; } = hostPropertyRepository;

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
