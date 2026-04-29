using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface ICropsRepository
    {
        Task AddAsync(Crop crop, CancellationToken ct = default);
        Task<Crop?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Crop>> GetByFieldIdAsync(Guid fieldId, CancellationToken ct = default);
        void Update(Crop crop);
        void Delete(Crop crop);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
