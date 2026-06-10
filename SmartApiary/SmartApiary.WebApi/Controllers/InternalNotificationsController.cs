using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Interfaces.Email;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Services;
using SmartApiary.Domain.Models;
using SmartApiary.WebApi.DTOs;

namespace SmartApiary.WebApi.Controllers
{
    [ApiController]
    [Route("api/internal/notifications")]
    public class InternalNotificationsController(
        IScaleNotificationService notificationService,
        ILogger<InternalNotificationsController> _logger
    ) : ControllerBase
    {
        [HttpPost("telemetry")]
        public async Task<IActionResult> SendRealtimeTelemetry([FromBody] InternalTelemetryPayload payload)
        {
            var telemetryResult = Telemetry.Create(
                payload.SmartScaleId,
                payload.Weight,
                payload.Humidity,
                payload.Temperature,
                payload.BatteryLevel,
                payload.Timestamp
            );

            if (telemetryResult.IsFailure)
            {
                return BadRequest(telemetryResult.Error);
            }

            await notificationService.SendRealtimeTelemetryAsync(payload.ApiaryId, telemetryResult.Value!);

            return Ok(new { message = "SignalR real-time telemetry broadcasted successfully." });
        }

        [HttpPost("anomaly")]
        public async Task<IActionResult> HandleAnomaly([FromBody] InternalAnomalyPayload payload)
        {
            await notificationService.SendAnomalyNotificationAsync(payload.ApiaryId, payload.ScaleId, payload.AnomalyType, payload.Message);
            return Ok();
        }
    }
}
