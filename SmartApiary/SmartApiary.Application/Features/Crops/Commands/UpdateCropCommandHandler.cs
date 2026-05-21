using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Crops.Commands
{
    public class UpdateCropCommandHandler : IRequestHandler<UpdateCropCommand, Result>
    {
        private readonly ICropsRepository _cropsRepository;
        

        public UpdateCropCommandHandler(
            ICropsRepository cropsRepository)
        {
            _cropsRepository = cropsRepository;
        }

        public async Task<Result> Handle(UpdateCropCommand request, CancellationToken cancellationToken)
        {
            var crop = await _cropsRepository.GetByIdAsync(request.Id, cancellationToken);
            if (crop is null)
                return Result.Failure("Crop not found.");

            try
            {
                crop.Update(
                    request.CropName,
                    request.Description,
                    request.FloweringPeriodStart,
                    request.FloweringPeriodEnd);
            }
            catch (ArgumentException ex)
            {
                return Result.Failure(ex.Message);
            }

            _cropsRepository.Update(crop);
            await _cropsRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
