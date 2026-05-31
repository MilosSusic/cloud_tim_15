using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Hives.Queries
{
    public class GetHivesByApiaryQueryHandler : IRequestHandler<GetHivesByApiaryQuery, Result<List<HiveDto>>>
    {
        private readonly IHiveRepository _hiveRepository;
        private readonly IApiaryRepository _apiaryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ISmartScaleRepository _smartScaleRepository;

        public GetHivesByApiaryQueryHandler(
            IHiveRepository hiveRepository,
            IApiaryRepository apiaryRepository,
            ICurrentUserService currentUserService,
            ISmartScaleRepository smartScaleRepository)
        {
            _hiveRepository = hiveRepository;
            _apiaryRepository = apiaryRepository;
            _currentUserService = currentUserService;
            _smartScaleRepository = smartScaleRepository;
        }

        public async Task<Result<List<HiveDto>>> Handle(GetHivesByApiaryQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Result<List<HiveDto>>.Failure("Invalid user id.");

            var apiary = await _apiaryRepository.GetByIdAsync(request.ApiaryId, cancellationToken);
            if (apiary is null)
                return Result<List<HiveDto>>.Failure("Apiary not found.");

            if (apiary.OwnerId != userId.Value)
                return Result<List<HiveDto>>.Failure("You do not have access to this apiary.");

            var hives = await _hiveRepository.GetByApiaryIdAsync(request.ApiaryId, cancellationToken);

            var allScales = await _smartScaleRepository.GetAllAsync(cancellationToken);

            //var result = hives
            //    .Select(h => new HiveDto(h.Id, h.ApiaryId, h.Label, h.Type, h.Color, h.QueenAge, h.Note))
            //    .ToList();
            var result = hives
                .Select(h => {
                    var associatedScale = allScales.FirstOrDefault(s => s.HiveId == h.Id);

                    return new HiveDto(
                        h.Id,
                        h.ApiaryId,
                        h.Label,
                        h.Type,
                        h.Color,
                        h.QueenAge,
                        h.Note,
                        associatedScale?.SerialNumber
                    );
                })
                .ToList();

            return Result<List<HiveDto>>.Success(result);
        }
    }
}
