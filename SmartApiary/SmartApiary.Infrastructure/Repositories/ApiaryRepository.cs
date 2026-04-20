using Microsoft.EntityFrameworkCore;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence;

namespace SmartApiary.Infrastructure.Repositories
{
    public class ApiaryRepository : IApiaryRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ApiaryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Apiary apiary, CancellationToken ct = default)
        {
            await _dbContext.Apiaries.AddAsync(apiary, ct);
        }

        public async Task<Apiary?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.Apiaries.Include(a => a.Hives).FirstOrDefaultAsync(a => a.Id == id, ct);
        }

        public async Task<List<Apiary>> GetByOwnerIdAsync(Guid ownerId, CancellationToken ct = default)
        {
            return await _dbContext.Apiaries.Where(a => a.OwnerId == ownerId).ToListAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
