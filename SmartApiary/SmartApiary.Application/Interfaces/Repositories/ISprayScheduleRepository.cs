using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface ISprayScheduleRepository
    {
        Task AddAsync(SpraySchedule spraySchedule, CancellationToken ct = default);
        Task<SpraySchedule?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<SpraySchedule>> GetByFieldIdAsync(Guid fieldId, CancellationToken ct = default);
        Task<List<SpraySchedule>> GetScheduledForTomorrowAsync(CancellationToken ct = default);
        void Update(SpraySchedule spraySchedule);
        Task SaveChangesAsync(CancellationToken ct = default);

        Task<List<SpraySchedule>> GetPastScheduledAsync(CancellationToken ct = default);
    }
}
