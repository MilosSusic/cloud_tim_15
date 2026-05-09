using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Features.SpraySchedule.Commands;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;
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

            var crop = Crop.Create(
                request.FieldId,
                request.CropName,
                request.Description,
                request.FloweringPeriodStart,
                request.FloweringPeriodEnd
            );

            await _ropsRepository.AddAsync(crop, cancellationToken);
            await _ropsRepository.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(crop.Id);
        }
    }
}
