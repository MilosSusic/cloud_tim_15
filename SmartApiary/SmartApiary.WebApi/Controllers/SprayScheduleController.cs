using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.Fields.Commands;
using SmartApiary.Application.Features.Fields.Queries;
using SmartApiary.Application.Features.SpraySchedule.Commands;
using SmartApiary.Application.Features.SpraySchedule.Queries;

namespace SmartApiary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Farmer")]
    public class SprayScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SprayScheduleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSprayScheduleCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpGet("by-field/{fieldId:guid}")]
        public async Task<IActionResult> GetAll(Guid fieldId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetSprayScheduleForFieldQuery(fieldId), ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSprayScheduleCommand command, CancellationToken ct)
        {
            if (id != command.Id)
                return BadRequest("Route id and body id do not match.");

            var result = await _mediator.Send(command, ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return NoContent();
        }
        [HttpPut("reschedule/{id:guid}")]
        public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleSprayCommand command, CancellationToken ct)
        {
            var enrichedCommand = command with { Id = id };

            var result = await _mediator.Send(enrichedCommand, ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return NoContent();
        }

        [HttpPut("status/{id:guid}")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeSprayStatusCommand command, CancellationToken ct)
        {
            var enrichedCommand = command with { Id = id };

            var result = await _mediator.Send(enrichedCommand, ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return NoContent();
        }

    }
}
