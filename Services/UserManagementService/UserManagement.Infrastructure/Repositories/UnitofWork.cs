using UserManagement.Infrastructure.Data;
using UserManagement.Infrastructure.Repositories.IRepositories;

namespace UserManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly UserManagementDbContext _context;

    public IUserRepository Users { get; }
    public IRoleRepository Roles { get; }
    public IHostPropertyRepository HostProperties { get; }

    public UnitOfWork(UserManagementDbContext context, IUserRepository userRepository, IRoleRepository roleRepository, IHostPropertyRepository hostPropertyRepository)
    {
        _context = context;
        Users = userRepository;
        Roles = roleRepository;
        HostProperties = hostPropertyRepository;
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
