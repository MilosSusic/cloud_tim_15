using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Interfaces.Services
{
    public interface IScaleNotificationService
    {
        Task SendRealtimeTelemetryAsync(Guid apiaryId, Telemetry telemetry);
        Task SendAnomalyNotificationAsync(Guid apiaryId, Guid scaleId, string anomalyType, string message);
    }
}
