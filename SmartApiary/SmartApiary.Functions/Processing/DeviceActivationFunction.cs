using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Features.SmartScales.Commands;

namespace SmartApiary.Functions.Processing
{
    internal class DeviceActivationFunction(
        ILogger<DeviceActivationFunction> logger,
        IMediator _mediator)
    {
        [Function("ActivateDevice")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "devices/activate")] HttpRequestData req)
        {
            logger.LogInformation("Processing IoT device activation request via Isolated Worker.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<ActivationRequestDto>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Da ne pravi problem ako simulator pošalje mala/velika slova
            });

            if (data == null)
            {
                var badResp = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResp.WriteAsJsonAsync(new { Message = "Request body cannot be empty." });
                return badResp;
            }

            var command = new ActivateScaleCommand(data.SerialNumber, data.HardwareId);

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                logger.LogWarning("Activation failed for S/N {SerialNumber}: {Error}", data.SerialNumber, result.Error);

                var badResp = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResp.WriteAsJsonAsync(result.Error);
                return badResp;
            }

            logger.LogInformation("Device S/N {SerialNumber} successfully activated and paired.", data.SerialNumber);

            var resp = req.CreateResponse(HttpStatusCode.OK);
            await resp.WriteAsJsonAsync(new
            {
                Message = "Device successfully activated.",
                Data = new { AccessToken = result.Value }
            });

            return resp;
        }
    }
    public class ActivationRequestDto
    {
        public string SerialNumber { get; set; } = string.Empty;
        public Guid HardwareId { get; set; }
    }
}