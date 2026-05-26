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
        Task<bool> IsSerialNumberUniqueAsync(string serialNumber, CancellationToken ct = default);
        void Update(SmartScale device);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
