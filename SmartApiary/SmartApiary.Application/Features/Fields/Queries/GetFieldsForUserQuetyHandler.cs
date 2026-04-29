using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Features.Apiaries.Queries;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Fields.Queries
{
    public class GetFieldsForUserQueryHandler : IRequestHandler<GetFieldsForUserQuery, Result<List<FieldListItemDto>>>
    {
        private readonly IFieldRepository _fieldRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetFieldsForUserQueryHandler(IFieldRepository fieldRepository, IHttpContextAccessor httpContextAccessor)
        {
            _fieldRepository = fieldRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<List<FieldListItemDto>>> Handle(GetFieldsForUserQuery request, CancellationToken cancellationToken)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Result<List<FieldListItemDto>>.Failure("Invalid user id.");  
            var fields = await _fieldRepository.GetByOwnerIdAsync(userId, cancellationToken);
            var dtos = fields.Select(f => new FieldListItemDto
            {
                Id = f.Id,
                FieldName = f.FieldName,
                Description = f.Description,
                Latitude = f.Latitude,
                Longitude = f.Longitude,
                ImageUrl = f.ImageUrl,
                ThumbnailUrl = f.ThumbnailUrl
            }).ToList();
            return Result<List<FieldListItemDto>>.Success(dtos);
        }
    }
}
