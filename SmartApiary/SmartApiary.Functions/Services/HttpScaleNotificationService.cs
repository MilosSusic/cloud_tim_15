using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Services;
using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Functions.Services
{
    public class HttpScaleNotificationService : IScaleNotificationService
    {
        private readonly HttpClient _httpClient;

        public HttpScaleNotificationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendRealtimeTelemetryAsync(Guid apiaryId, Telemetry telemetry)
        {
            var payload = new
            {
                ApiaryId = apiaryId,
                SmartScaleId = telemetry.SmartScaleId.ToString(),
                Weight = telemetry.Weight.Value,
                Humidity = telemetry.Humidity.Value,
                Temperature = telemetry.Temperature.Value,
                BatteryLevel = telemetry.BatteryLevel.Value,
                Timestamp = telemetry.Timestamp
            };

            var response = await _httpClient.PostAsJsonAsync("api/internal/notifications/telemetry", payload);
            response.EnsureSuccessStatusCode();
        }

        public async Task SendAnomalyNotificationAsync(Guid apiaryId, Guid scaleId, string anomalyType, string message)
        {
            var payload = new
            {
                ApiaryId = apiaryId,
                ScaleId = scaleId,
                AnomalyType = anomalyType,
                Message = message,
                CurrentValue = 0.0
            };

            var response = await _httpClient.PostAsJsonAsync("api/internal/notifications/anomaly", payload);
            response.EnsureSuccessStatusCode();            
        }

        public async Task SendYieldDeltaNotificationAsync(Guid apiaryId, string scaleId, double deltaKg, DateTime recordedAt)
        {
            var payload = new
            {
                ApiaryId = apiaryId,
                ScaleId = scaleId,
                DeltaKg = deltaKg,
                RecordedAt = recordedAt
            };

            var response = await _httpClient.PostAsJsonAsync("api/internal/notifications/yield", payload);
            response.EnsureSuccessStatusCode();
        }
    }
}
