using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Users.Commands
{
    public class SuspendUserCommandHandler(IUserRepository userRepository)
        : IRequestHandler<SuspendUserCommand, Result>
    {
        public async Task<Result> Handle(SuspendUserCommand request, CancellationToken ct)
        {
            var user = await userRepository.GetByEmailAsync(request.Email, ct);

            if (user is null)
            {
                return Result.Failure("Korisnik nije pronađen.");
            }

            user.Suspend();

            await userRepository.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}