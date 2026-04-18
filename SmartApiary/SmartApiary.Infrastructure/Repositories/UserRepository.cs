using Microsoft.EntityFrameworkCore;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence;

namespace SmartApiary.Infrastructure.Repositories
{
    public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
    {
        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await dbContext.Users.AddAsync(user, ct);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, ct);
        }

        public async Task<User?> GetByActivationTokenAsync(string activationToken, CancellationToken ct = default)
        {
            return await dbContext.Users.FirstOrDefaultAsync(x => x.ActivationToken == activationToken, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await dbContext.SaveChangesAsync(ct);
        }
        public async Task<User?> GetByResetPasswordTokenAsync(string resetPasswordToken, CancellationToken ct = default)
        {
            return await dbContext.Users.FirstOrDefaultAsync(x => x.ResetPasswordToken == resetPasswordToken, ct);
        }
        public async Task<List<User>> GetAllAsync(CancellationToken ct = default)
        {
            return await dbContext.Users.ToListAsync(ct);
        }
    }
}