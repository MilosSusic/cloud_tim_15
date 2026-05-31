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
                PropertyNameCaseInsensitive = true
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
                logger.LogWarning("Activation handler returned failure for S/N {SerialNumber}: {Error}", data.SerialNumber, result.Error);

                // Ako je u pitanju signal da je potreban Hardware ID, vraćamo poseban status (npr. 428 Precondition Required)
                if (result.Error == "HARDWARE_ID_REQUIRED")
                {
                    var signalResp = req.CreateResponse(HttpStatusCode.PreconditionRequired); // Status 428
                    await signalResp.WriteAsJsonAsync(new { Message = "HARDWARE_ID_REQUIRED" });
                    return signalResp;
                }

                // Za sve ostale prave greške (npr. vaga ne postoji) vraćamo BadRequest
                var badResp = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResp.WriteAsJsonAsync(new { Message = result.Error });
                return badResp;
            }

            logger.LogInformation("Device S/N {SerialNumber} processing successful.", data.SerialNumber);

            var resp = req.CreateResponse(HttpStatusCode.OK);
            await resp.WriteAsJsonAsync(new
            {
                Message = "Success",
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