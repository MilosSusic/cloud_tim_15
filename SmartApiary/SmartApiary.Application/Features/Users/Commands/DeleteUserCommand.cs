using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Users.Commands
{
    public record DeleteUserCommand(string Email) : IRequest<Result>;
}