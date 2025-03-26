namespace UserManagement.Infrastructure.Repositories.IRepositories;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IHostPropertyRepository HostProperties { get; }
    Task<int> SaveChangesAsync();
}
