using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.SmartScales.Commands;
using SmartApiary.Application.Features.SmartScales.Queries;

namespace SmartApiary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Beekeeper")]
    public class SmartScalesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SmartScalesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterScaleCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(new { Message = "Scale successfully registered with status: 'Unpaired'.", ScaleId = result.Value });
        }

        [HttpGet("analytics/apiary/{apiaryId:guid}")]
        public async Task<IActionResult> GetApiaryAnalytics(Guid apiaryId, [FromQuery] int days = 7, CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetApiaryAnalyticsQuery(apiaryId, days), ct);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }
}
