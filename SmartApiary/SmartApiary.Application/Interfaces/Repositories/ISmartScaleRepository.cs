using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface ISmartScaleRepository
    {
        Task AddAsync(SmartScale device, CancellationToken ct = default);
        Task<SmartScale?> GetBySerialNumberAsync(string serialNumber, CancellationToken ct = default);
        Task<SmartScale?> GetByTokenAsync(string token, CancellationToken ct = default);
        Task<SmartScale?> GetByHiveIdAsync(Guid hiveId, CancellationToken ct);
        Task<IEnumerable<SmartScale>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> IsSerialNumberUniqueAsync(string serialNumber, CancellationToken ct = default);
        void Update(SmartScale device);
        Task SaveChangesAsync(CancellationToken ct = default);

        Task<SmartScale?> GetByApiaryIdAsync(Guid apiaryId, CancellationToken ct = default);
        Task<Guid> GetApiaryIdByHiveIdAsync(Guid hiveId, CancellationToken ct);
        Task<SmartScale?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }
}
