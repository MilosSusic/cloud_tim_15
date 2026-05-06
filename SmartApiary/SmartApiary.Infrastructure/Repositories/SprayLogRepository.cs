using Microsoft.EntityFrameworkCore;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence;

namespace SmartApiary.Infrastructure.Repositories
{
    public class SprayLogRepository : ISprayLogRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SprayLogRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(SprayLog sprayLog, CancellationToken ct = default)
        {
            await _dbContext.SprayLogs.AddAsync(sprayLog, ct);
        }

        public async Task<List<SprayLog>> GetByFilterAsync(
            Guid? fieldId, DateTime? from, DateTime? to, CancellationToken ct = default)
        {
            var query = _dbContext.SprayLogs.AsQueryable();

            if (fieldId.HasValue)
                query = query.Where(x => x.FieldId == fieldId.Value);

            if (from.HasValue)
                query = query.Where(x => x.StartDateTime >= from.Value);

            if (to.HasValue)
                query = query.Where(x => x.StartDateTime <= to.Value);

            return await query
                .OrderByDescending(x => x.StartDateTime)
                .ToListAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}