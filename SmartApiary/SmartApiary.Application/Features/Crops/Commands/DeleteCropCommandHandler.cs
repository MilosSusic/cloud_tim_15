using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Features.Hives.Commands;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Crops.Commands
{
    public class DeleteCropCommandHandler : IRequestHandler<DeleteCropCommand, Result>
    {
        private readonly ICropsRepository _cropsRepository;
        private readonly IFieldRepository _fieldRepository;
        

        public DeleteCropCommandHandler(
            ICropsRepository cropsRepository,
            IFieldRepository fieldRepository)
        {
            _cropsRepository = cropsRepository;
            _fieldRepository = fieldRepository;
            
        }

        public async Task<Result> Handle(DeleteCropCommand request, CancellationToken cancellationToken)
        {
            

            var crop = await _cropsRepository.GetByIdAsync(request.Id, cancellationToken);
            if (crop is null)
                return Result.Failure("Crop not found.");

            var field = await _fieldRepository.GetByIdAsync(crop.FieldId, cancellationToken);
            if (field is null)
                return Result.Failure("Field not found.");
            crop.IsDeleted = true;

            _cropsRepository.Update(crop);
            await _cropsRepository.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
