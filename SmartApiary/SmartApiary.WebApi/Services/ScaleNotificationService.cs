using Microsoft.AspNetCore.SignalR;
using SmartApiary.Application.Interfaces.Services;
using SmartApiary.Domain.Models;
using SmartApiary.WebApi.Hubs;

namespace SmartApiary.WebApi.Services
{
    public class ScaleNotificationService(IHubContext<SmartScaleHub> hubContext, ILogger<ScaleNotificationService> _logger) : IScaleNotificationService
    {
        public async Task SendRealtimeTelemetryAsync(Guid apiaryId, Telemetry telemetry)
        {
            await hubContext.Clients.Group(apiaryId.ToString()).SendAsync("ReceiveScaleTelemetry", new
            {
                scaleId = telemetry.SmartScaleId.ToString(),
                weight = telemetry.Weight.Value,
                humidity = telemetry.Humidity.Value,
                temperature = telemetry.Temperature.Value,
                batteryLevel = telemetry.BatteryLevel.Value,
                timestamp = telemetry.Timestamp
            });
        }

        public async Task SendAnomalyNotificationAsync(Guid apiaryId, Guid scaleId, string anomalyType, string message)
        {
            await hubContext.Clients.Group(apiaryId.ToString()).SendAsync("ReceiveScaleAnomaly", new
            {
                scaleId = scaleId,
                type = anomalyType,
                message = message,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
