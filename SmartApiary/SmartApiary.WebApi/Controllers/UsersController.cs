using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.Users.Commands;

namespace SmartApiary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateUserCommand command,
            CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost("suspend")]
        public async Task<IActionResult> SuspendUser(
            [FromBody] SuspendUserCommand command,
            CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUser(
            [FromBody] DeleteUserCommand command,
            CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }
    }
}