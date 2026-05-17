using Microsoft.EntityFrameworkCore;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence;

namespace SmartApiary.Infrastructure.Repositories
{
    public class SprayScheduleRepository : ISprayScheduleRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public SprayScheduleRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(SpraySchedule spraySchedule, CancellationToken ct = default)
        {
            await _dbContext.SpraySchedules.AddAsync(spraySchedule, ct);
        }

        public async Task<SpraySchedule?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.SpraySchedules.FirstOrDefaultAsync(f => f.Id == id, ct);
        }

        public async Task<List<SpraySchedule>> GetByFieldIdAsync(Guid fieldId, CancellationToken ct = default)
        {
            return await _dbContext.SpraySchedules.Where(s => s.FieldId == fieldId).ToListAsync(ct);
        }
        public async Task<List<SpraySchedule>> GetScheduledForTomorrowAsync(CancellationToken ct = default)
        {
            var tomorrow = DateTime.UtcNow.AddDays(1).Date;
            var dayAfter = tomorrow.AddDays(1);

            return await _dbContext.SpraySchedules
                .Where(s => s.StartDateTime >= tomorrow
                         && s.StartDateTime < dayAfter
                         && s.Status == SprayStatus.Scheduled)
                .ToListAsync(ct);
        }

        public async Task<List<SpraySchedule>> GetPastScheduledAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            return await _dbContext.SpraySchedules
                .Where(s => (s.Status == SprayStatus.Scheduled || s.Status == SprayStatus.Rescheduled)
                         && s.StartDateTime.AddHours(s.DurationHours) < now)
                .ToListAsync(ct);
        }

        public void Update(SpraySchedule spraySchedule)
        {
            _dbContext.SpraySchedules.Update(spraySchedule);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
