using Microsoft.EntityFrameworkCore;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence;

namespace SmartApiary.Infrastructure.Repositories
{
    public class HiveLogEntryRepository : IHiveLogEntryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public HiveLogEntryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(HiveLogEntry entry, CancellationToken ct = default)
        {
            await _dbContext.HiveLogEntries.AddAsync(entry, ct);
        }

        public async Task<HiveLogEntry?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.HiveLogEntries
                .FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public async Task<List<HiveLogEntry>> GetByHiveIdAsync(Guid hiveId, CancellationToken ct = default)
        {
            return await _dbContext.HiveLogEntries
                .Where(e => e.HiveId == hiveId)
                .OrderByDescending(e => e.ReviewDate)
                .ToListAsync(ct);
        }

        public async Task<(List<HiveLogEntry> Items, int TotalCount)> GetPagedByHiveIdAsync(
            Guid hiveId,
            int page,
            int pageSize,
            CancellationToken ct = default)
        {
            var query = _dbContext.HiveLogEntries
                .Where(e => e.HiveId == hiveId);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(e => e.ReviewDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public void Update(HiveLogEntry entry)
        {
            _dbContext.HiveLogEntries.Update(entry);
        }

        public void Delete(HiveLogEntry entry)
        {
            _dbContext.HiveLogEntries.Remove(entry);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
