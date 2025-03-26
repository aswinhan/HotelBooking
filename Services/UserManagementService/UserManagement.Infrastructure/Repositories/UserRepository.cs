﻿using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Data;
using UserManagement.Infrastructure.Repositories.IRepositories;

namespace UserManagement.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(UserManagementDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email) => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    public async Task<User?> GetWithProfileAsync(Guid userId) => await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Id == userId);
}
