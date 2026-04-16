using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.Users.Commands;
using SmartApiary.Application.Features.Users.Queries;

namespace SmartApiary.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        [HttpPost("activate")]
        public async Task<IActionResult> Activate(ActivateAccountCommand command, CancellationToken ct)
        {
            var result = await mediator.Send(command, ct);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginQuery query, CancellationToken ct)
        {
            var result = await mediator.Send(query, ct);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command, CancellationToken ct)
        {
            var result = await mediator.Send(command, ct);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command, CancellationToken ct)
        {
            var result = await mediator.Send(command, ct);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }
    }
}