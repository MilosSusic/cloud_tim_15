using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.Fields.Queries;

namespace SmartApiary.WebApi.Controllers
{
    [ApiController]
    [Route("api/beekeeper")]
    [Authorize(Roles = "Beekeeper")]
    public class BeekeeperFieldsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BeekeeperFieldsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("nearby-fields-with-crops")]
        public async Task<IActionResult> GetNearbyFieldsWithCrops(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetNearbyFieldsWithCropsQuery(Guid.Empty), ct);
            return Ok(result);
        }
    }
}
