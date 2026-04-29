using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.Crops.Commands;
using SmartApiary.Application.Features.Crops.Queries;
using SmartApiary.Application.Features.Fields.Commands;
using SmartApiary.Application.Features.Fields.Queries;
using SmartApiary.Application.Features.Hives.Commands;

namespace SmartApiary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Farmer")]
    public class CropController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CropController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCropCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpGet("by-field/{fieldId:guid}")]
        public async Task<IActionResult> GetAllFromField(Guid fieldId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetCropsForFieldIdQuery(fieldId), ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromBody] UpdateCropCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeleteCropCommand(id), ct);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return NoContent();
        }
    }
}
