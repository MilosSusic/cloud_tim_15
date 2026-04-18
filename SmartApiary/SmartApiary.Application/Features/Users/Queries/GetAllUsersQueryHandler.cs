using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.Users.Queries
{
    public class GetAllUsersQueryHandler(IUserRepository userRepository)
        : IRequestHandler<GetAllUsersQuery, Result<List<User>>>
    {
        public async Task<Result<List<User>>> Handle(GetAllUsersQuery request, CancellationToken ct)
        {
            var users = await userRepository.GetAllAsync(ct);
            return Result<List<User>>.Success(users);
        }
    }
}