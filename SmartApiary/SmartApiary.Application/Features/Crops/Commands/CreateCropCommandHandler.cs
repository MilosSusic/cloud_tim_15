using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Features.SpraySchedule.Commands;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var crop = new Domain.Models.Crop
            {
                Id = Guid.NewGuid(),
                FieldId = request.FieldId,
                CropName = request.CropName,
                Description = request.Description,
                FloweringPeriodStart = request.FloweringPeriodStart,
                FloweringPeriodEnd = request.FloweringPeriodEnd,
                IsDeleted = false
            };

            if (crop.FloweringPeriodStart > crop.FloweringPeriodEnd)
            {
                var temp = crop.FloweringPeriodStart;
                crop.FloweringPeriodStart = crop.FloweringPeriodEnd;
                crop.FloweringPeriodEnd = temp;
            }

            await _ropsRepository.AddAsync(crop, cancellationToken);
            await _ropsRepository.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(crop.Id);
        }
    }
}
