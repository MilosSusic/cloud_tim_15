using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface IHiveRepository
    {
        Task AddAsync(Hive hive, CancellationToken ct = default);
        Task<Hive?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Hive>> GetByApiaryIdAsync(Guid apiaryId, CancellationToken ct = default);
        void Update(Hive hive);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
