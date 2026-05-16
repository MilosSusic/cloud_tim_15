using Microsoft.EntityFrameworkCore;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence;

namespace SmartApiary.Infrastructure.Repositories
{
    public class HiveRepository : IHiveRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public HiveRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Hive hive, CancellationToken ct = default)
        {
            await _dbContext.Hives.AddAsync(hive, ct);
        }

        public async Task<Hive?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.Hives
                .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted, ct);
        }

        public async Task<List<Hive>> GetByApiaryIdAsync(Guid apiaryId, CancellationToken ct = default)
        {
            return await _dbContext.Hives
                .Where(h => h.ApiaryId == apiaryId && !h.IsDeleted)
                .ToListAsync(ct);
        }

        public void Update(Hive hive)
        {
            _dbContext.Hives.Update(hive);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
