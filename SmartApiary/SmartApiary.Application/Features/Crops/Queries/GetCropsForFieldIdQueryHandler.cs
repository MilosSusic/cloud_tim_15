using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Crops.Queries
{
    public class GetCropsForFieldIdQueryHandler : IRequestHandler<GetCropsForFieldIdQuery, Result<List<CropListItemDto>>>
    {
        private readonly ICropsRepository _cropsRepository;

        public GetCropsForFieldIdQueryHandler(ICropsRepository cropsRepository)
        {
            _cropsRepository = cropsRepository;
        }

        public async Task<Result<List<CropListItemDto>>> Handle(GetCropsForFieldIdQuery request, CancellationToken cancellationToken)
        {
            var crops = await _cropsRepository.GetByFieldIdAsync(request.FieldId, cancellationToken);
            var dtos = crops
                .Select(c => new CropListItemDto
            {
                Id = c.Id,
                CropName = c.CropName,
                Description = c.Description,
                FloweringPeriodStart = c.FloweringPeriodStart,
                FloweringPeriodEnd = c.FloweringPeriodEnd,
                IsDeleted = c.IsDeleted

                }).ToList();
            return Result<List<CropListItemDto>>.Success(dtos);
        }
    }
}
