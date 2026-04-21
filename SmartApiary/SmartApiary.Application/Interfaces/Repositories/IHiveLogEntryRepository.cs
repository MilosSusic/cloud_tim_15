using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface IHiveLogEntryRepository
    {
        Task AddAsync(HiveLogEntry entry, CancellationToken ct = default);
        Task<HiveLogEntry?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<HiveLogEntry>> GetByHiveIdAsync(Guid hiveId, CancellationToken ct = default);
        Task<(List<HiveLogEntry> Items, int TotalCount)> GetPagedByHiveIdAsync(
            Guid hiveId,
            int page,
            int pageSize,
            CancellationToken ct = default);
        void Update(HiveLogEntry entry);
        void Delete(HiveLogEntry entry);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
