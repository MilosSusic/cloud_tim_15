using MediatR;
using SmartApiary.Domain.Common;
using SmartApiary.Application.Interfaces.Repositories;

namespace SmartApiary.Application.Features.SpraySchedule.Queries
{
    public class GetSprayScheduleForFieldQueryHandler : IRequestHandler<GetSprayScheduleForFieldQuery, Result<List<SprayListItemDto>>>
    {
        private readonly ISprayScheduleRepository _sprayScheduleRepository;

        public GetSprayScheduleForFieldQueryHandler(ISprayScheduleRepository sprayScheduleRepository)
        {
            _sprayScheduleRepository = sprayScheduleRepository;
        }

        public async Task<Result<List<SprayListItemDto>>> Handle(GetSprayScheduleForFieldQuery request, CancellationToken cancellationToken)
        {
            var fields = await _sprayScheduleRepository.GetByFieldIdAsync(request.FieldId, cancellationToken);
            var dtos = fields.Select(f => new SprayListItemDto
            {
                Id = f.Id,
                FieldId = f.FieldId,
                StartDateTime = f.StartDateTime,
                DurationHours = f.DurationHours,
                PesticideType = f.PesticideType,
                Status = f.Status,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            }).ToList();
            return Result<List<SprayListItemDto>>.Success(dtos);
        }
    }
}
