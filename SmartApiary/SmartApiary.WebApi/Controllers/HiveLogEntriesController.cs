using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.HiveLogEntries.Commands;
using SmartApiary.Application.Features.HiveLogEntries.Queries;

namespace SmartApiary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Beekeeper")]
    public class HiveLogEntriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HiveLogEntriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHiveLogEntryCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpGet("by-hive/{hiveId:guid}")]
        public async Task<IActionResult> GetByHive(
            Guid hiveId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetHiveLogEntriesByHiveQuery(hiveId, page, pageSize), ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHiveLogEntryCommand command, CancellationToken ct)
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
            var result = await _mediator.Send(new DeleteHiveLogEntryCommand(id), ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return NoContent();
        }
    }
}
