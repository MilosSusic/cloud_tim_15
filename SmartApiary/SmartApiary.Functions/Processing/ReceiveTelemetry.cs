using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Features.SmartScales.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartApiary.Functions.Processing
{
    public class ReceiveTelemetry(IMediator mediator, ILogger<ReceiveTelemetry> logger)
    {
        [Function("ReceiveTelemetry")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "scales/telemetry")] HttpRequestData req)
        {
            logger.LogInformation("[API] Received telemetry payload request.");

            if (!req.Headers.TryGetValues("X-Device-Token", out var tokenValues))
            {
                var unauthorizedResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await unauthorizedResponse.WriteStringAsync("Missing X-Device-Token header.");
                return unauthorizedResponse;
            }
            string token = tokenValues.First();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var telemetryDto = JsonSerializer.Deserialize<TelemetryIncomingDto>(requestBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (telemetryDto is null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var command = new ProcessScaleTelemetryCommand
            {
                DeviceToken = token,
                Weight = telemetryDto.Weight,
                Humidity = telemetryDto.Humidity,
                Temperature = telemetryDto.Temperature,
                BatteryLevel = telemetryDto.BatteryLevel,
                Timestamp = DateTime.UtcNow
            };

            var result = await mediator.Send(command);

            var response = req.CreateResponse();
            if (result.IsSuccess)
            {
                response.StatusCode = HttpStatusCode.OK;
                await response.WriteStringAsync("Telemetry accepted.");
            }
            else
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync(result.Error ?? "Error processing telemetry.");
            }

            return response;
        }

        private class TelemetryIncomingDto
        {
            public double Weight { get; set; }
            public double Humidity { get; set; }
            public double Temperature { get; set; }
            public double BatteryLevel { get; set; }
            //public DateTime? Timestamp { get; set; }
        }
    }
}
