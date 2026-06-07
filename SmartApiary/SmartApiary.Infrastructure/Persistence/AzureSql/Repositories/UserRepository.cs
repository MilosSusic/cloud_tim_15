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
        public void Remove(User user)
        {
            dbContext.Users.Remove(user);
        }
        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
        }
        public async Task<User?> GetOwnerByScaleIdAsync(Guid scaleId, CancellationToken ct = default)
        {
            var scale = await dbContext.SmartScales
                .FirstOrDefaultAsync(s => s.Id == scaleId, ct);

            if (scale == null) return null;

            return await dbContext.Users
                .FirstOrDefaultAsync(u => dbContext.Apiaries
                    .Any(a => a.OwnerId == u.Id && dbContext.Hives
                        .Any(h => h.Id == scale.HiveId && h.ApiaryId == a.Id)), ct);
        }
    }
}