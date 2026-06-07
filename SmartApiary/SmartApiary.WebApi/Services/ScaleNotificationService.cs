using Microsoft.AspNetCore.SignalR;
using SmartApiary.Application.Interfaces.Services;
using SmartApiary.Domain.Models;
using SmartApiary.WebApi.Hubs;

namespace SmartApiary.WebApi.Services
{
    public class ScaleNotificationService(IHubContext<SmartScaleHub> hubContext) : IScaleNotificationService
    {
        public async Task SendRealtimeTelemetryAsync(Telemetry telemetry)
        {
            await hubContext.Clients.All.SendAsync("ReceiveScaleTelemetry", new
            {
                scaleId = telemetry.SmartScaleId.ToString(),
                weight = telemetry.Weight.Value,
                humidity = telemetry.Humidity.Value,
                temperature = telemetry.Temperature.Value,
                batteryLevel = telemetry.BatteryLevel.Value,
                timestamp = telemetry.Timestamp
            });
        }

        public async Task SendAnomalyNotificationAsync(Guid scaleId, string anomalyType, string message)
        {
            await hubContext.Clients.All.SendAsync("ReceiveScaleAnomaly", new
            {
                scaleId,
                type = anomalyType,
                message
            });
        }
    }
}
