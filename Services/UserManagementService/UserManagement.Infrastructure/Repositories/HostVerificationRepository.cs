using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Data;
using UserManagement.Infrastructure.Repositories.IRepositories;

namespace UserManagement.Infrastructure.Repositories;

public class HostVerificationRepository(UserManagementDbContext context) : GenericRepository<HostVerification>(context), IHostVerificationRepository
{
    public async Task<HostVerification?> GetByUserIdAsync(Guid userId) => await _context.HostVerifications.FirstOrDefaultAsync(r => r.UserId == userId);
}
