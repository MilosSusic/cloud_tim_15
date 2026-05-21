using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.Crops.Commands
{
    public class CreateCropCommandHandler : IRequestHandler<CreateCropCommand, Result<Guid>>
    {
        private readonly ICropsRepository _ropsRepository;
        public CreateCropCommandHandler(ICropsRepository cropRepository)
        {
            _ropsRepository = cropRepository;
        }

        public async Task<Result<Guid>> Handle(CreateCropCommand request, CancellationToken cancellationToken)
        {
            Crop crop;
            try
            {
                crop = Crop.Create(
                    request.FieldId,
                    request.CropName,
                    request.Description,
                    request.FloweringPeriodStart,
                    request.FloweringPeriodEnd);
            }
            catch (ArgumentException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }

            await _ropsRepository.AddAsync(crop, cancellationToken);
            await _ropsRepository.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(crop.Id);
        }
    }
}
