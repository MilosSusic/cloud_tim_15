using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.SmartScales.Commands;

namespace SmartApiary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class YieldRecordsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<YieldRecordsController> _logger;

        public YieldRecordsController(IMediator mediator, ILogger<YieldRecordsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveYieldRecord([FromBody] SaveYieldRecordRequest request)
        {
            _logger.LogInformation("[YIELD CONTROLLER] Received SaveYieldRecord request: ScaleId={ScaleId}, DeltaKg={Delta}, RecordedAt={Time}",
                request?.ScaleId, request?.DeltaKg, request?.RecordedAt);

            if (request == null)
            {
                _logger.LogWarning("[YIELD CONTROLLER] Request is null");
                return BadRequest("Invalid request.");
            }

            var command = new SaveYieldRecordCommand
            {
                ScaleId = request.ScaleId,
                DeltaKg = request.DeltaKg,
                RecordedAt = request.RecordedAt
            };

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                _logger.LogWarning("[YIELD CONTROLLER] Failed to save yield record: {Error}", result.Error);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("[YIELD CONTROLLER] Yield record saved successfully for ScaleId: {ScaleId}", request.ScaleId);
            return Ok("Yield record saved successfully.");
        }
    }

    public class SaveYieldRecordRequest
    {
        public string ScaleId { get; set; } = string.Empty;
        public double DeltaKg { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
