using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface IApiaryRepository
    {
        Task AddAsync(Apiary apiary, CancellationToken ct = default);
        Task<Apiary?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Apiary>> GetByOwnerIdAsync(Guid ownerId, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
