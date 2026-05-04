using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Features.Apiaries.Queries;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
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
        private readonly ICurrentUserService _currentUserService;

        public GetFieldsForUserQueryHandler(IFieldRepository fieldRepository, ICurrentUserService currentUserService)
        {
            _fieldRepository = fieldRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<FieldListItemDto>>> Handle(GetFieldsForUserQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Result<List<FieldListItemDto>>.Failure("Invalid user id.");
            var fields = await _fieldRepository.GetByOwnerIdAsync(userId.Value, cancellationToken);
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
