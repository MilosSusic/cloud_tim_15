using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Features.Apiaries.Commands;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Fields.Commands
{
    public class UpdateFieldCommandHandler : IRequestHandler<UpdateFieldCommand, Result>
    {
        private readonly IFieldRepository _fieldRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateFieldCommandHandler(
            IFieldRepository fieldRepository,
            ICurrentUserService currentUserService)
        {
            _fieldRepository = fieldRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(UpdateFieldCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Result<Guid>.Failure("Invalid user id.");

            var field = await _fieldRepository.GetByIdAsync(request.Id, cancellationToken);
            if (field is null)
                return Result.Failure("Field not found.");

            if (field.OwnerId != userId.Value)
                return Result.Failure("You do not have access to this field.");

            field.FieldName = request.FieldName;
            field.Description = request.Description;
            field.Latitude = request.Latitude;
            field.Longitude = request.Longitude;

            if (!string.IsNullOrWhiteSpace(request.ImageUrl))
                field.ImageUrl = request.ImageUrl;
            if (!string.IsNullOrWhiteSpace(request.ThumbnailUrl))
                field.ThumbnailUrl = request.ThumbnailUrl;

            _fieldRepository.Update(field);
            await _fieldRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
