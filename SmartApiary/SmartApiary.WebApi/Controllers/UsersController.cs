using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.Users.Commands;

namespace SmartApiary.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserCommand command, CancellationToken ct)
        {
            var result = await mediator.Send(command, ct);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }
}