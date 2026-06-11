using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface IYieldRecordRepository
    {
        Task SaveAsync(YieldRecord yieldRecord, CancellationToken ct);
        Task<IEnumerable<YieldRecord>> GetHistoryAsync(string scaleId, DateTime startFrom, CancellationToken ct = default);
    }
}
