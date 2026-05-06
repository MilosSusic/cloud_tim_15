using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface ISprayLogRepository
    {
        Task AddAsync(SprayLog sprayLog, CancellationToken ct = default);
        Task<List<SprayLog>> GetByFilterAsync(Guid? fieldId, DateTime? from, DateTime? to, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}