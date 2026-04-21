using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.Hives.Commands;
using SmartApiary.Application.Features.Hives.Queries;

namespace SmartApiary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Beekeeper")]
    public class HivesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HivesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHiveCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpGet("by-apiary/{apiaryId:guid}")]
        public async Task<IActionResult> GetByApiary(Guid apiaryId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetHivesByApiaryQuery(apiaryId), ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHiveCommand command, CancellationToken ct)
        {
            if (id != command.Id)
                return BadRequest("Route id and body id do not match.");

            var result = await _mediator.Send(command, ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeleteHiveCommand(id), ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return NoContent();
        }
    }
}
