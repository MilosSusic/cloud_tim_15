using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Users.Queries
{
    public record LoginQuery(
        string Email,
        string Password
    ) : IRequest<Result<string>>;
}