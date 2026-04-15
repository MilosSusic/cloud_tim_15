using MediatR;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;

namespace SmartApiary.Application.Features.Users.Commands
{
    public record CreateUserCommand(
        string FirstName,
        string LastName,
        string Email,
        UserRole Role
    ) : IRequest<Result<Guid>>;
}