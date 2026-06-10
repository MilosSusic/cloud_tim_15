using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface ITelemetryRepository
    {
        Task SaveAsync(Telemetry telemetry, CancellationToken ct);
        Task<Telemetry?> GetLatestBeforeAsync(string scaleId, DateTimeOffset timestamp, string currentRowKey, CancellationToken ct);
        Task<IEnumerable<Telemetry>> GetHistoryAsync(string scaleId, DateTime startFrom, CancellationToken ct = default);
    }
}
