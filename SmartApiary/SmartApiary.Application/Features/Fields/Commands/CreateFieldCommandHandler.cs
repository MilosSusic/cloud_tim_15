using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;

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

            var fieldResult = Field.Create(
                userId.Value,
                request.FieldName,
                request.Description,
                request.Latitude,
                request.Longitude,
                request.ImageUrl,
                request.ThumbnailUrl);

            if (fieldResult.IsFailure)
                return Result<Guid>.Failure(fieldResult.Error!);

            var field = fieldResult.Value!;

            await _fieldRepository.AddAsync(field, cancellationToken);
            await _fieldRepository.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(field.Id);
        }
    }
}
