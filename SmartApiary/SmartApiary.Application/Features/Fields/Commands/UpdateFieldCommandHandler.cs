using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Domain.Common;

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
                return Result.Failure("Invalid user id.");

            var field = await _fieldRepository.GetByIdAsync(request.Id, cancellationToken);
            if (field is null)
                return Result.Failure("Field not found.");

            if (field.OwnerId != userId.Value)
                return Result.Failure("You do not have access to this field.");

            var updateResult = field.Update(request.FieldName,
                                            request.Description,
                                            request.Latitude,
                                            request.Longitude,
                                            request.ImageUrl,
                                            request.ThumbnailUrl);

            if (updateResult.IsFailure)
                return Result.Failure(updateResult.Error!);

            _fieldRepository.Update(field);
            await _fieldRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
