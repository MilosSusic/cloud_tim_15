using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Features.Apiaries.Commands;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Fields.Commands
{
    public class CreateFieldCommandHandler : IRequestHandler<CreateFieldCommand, Result<Guid>>
    {
        private readonly IFieldRepository _fieldRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateFieldCommandHandler(IFieldRepository fieldRepository, ICurrentUserService currentUserService)
        {
            _fieldRepository = fieldRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(CreateFieldCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Result<Guid>.Failure("Invalid user id.");

            var field = new Field
            {
                Id = Guid.NewGuid(),
                FieldName = request.FieldName,
                Description = request.Description,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                ImageUrl = request.ImageUrl,
                ThumbnailUrl = request.ThumbnailUrl,
                OwnerId = userId.Value 
            };

            await _fieldRepository.AddAsync(field, cancellationToken);
            await _fieldRepository.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(field.Id);
        }
    }
}
