using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Repositories.IRepositories;

public interface IHostVerificationRepository : IGenericRepository<HostVerification>
{
    Task<HostVerification?> GetByUserIdAsync(Guid userId);
}

