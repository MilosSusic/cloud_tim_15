using MediatR;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.Users.Queries
{
    public record GetAllUsersQuery : IRequest<Result<List<User>>>;
}