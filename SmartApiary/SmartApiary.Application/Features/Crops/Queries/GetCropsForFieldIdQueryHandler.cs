using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Features.Fields.Queries;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
                .Where(c => !c.IsDeleted)
                .Select(c => new CropListItemDto
            {
                Id = c.Id,
                CropName = c.CropName,
                Description = c.Description,
                Note = c.Note,
                Color = c.Color
            }).ToList();
            return Result<List<CropListItemDto>>.Success(dtos);
        }
    }
}
