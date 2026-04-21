using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.Apiaries.Commands;
using SmartApiary.Application.Features.Apiaries.Queries;

namespace SmartApiary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Beekeeper")]
    public class ApiariesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApiariesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApiaryCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetApiariesForUserQuery(), ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateApiaryCommand command, CancellationToken ct)
        {
            if (id != command.Id)
                return BadRequest("Route id and body id do not match.");

            var result = await _mediator.Send(command, ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return NoContent();
        }
    }
}
