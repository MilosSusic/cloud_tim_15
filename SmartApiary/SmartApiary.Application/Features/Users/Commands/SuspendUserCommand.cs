using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Users.Commands
{
    public record SuspendUserCommand(string Email) : IRequest<Result>;
}