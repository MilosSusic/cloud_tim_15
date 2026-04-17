using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Users.Commands
{
    public class DeleteUserCommandHandler(IUserRepository userRepository)
        : IRequestHandler<DeleteUserCommand, Result>
    {
        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken ct)
        {
            var user = await userRepository.GetByEmailAsync(request.Email, ct);

            if (user is null || user.IsDeleted)
            {
                return Result.Failure("User not found.");
            }

            user.Delete();

            await userRepository.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}