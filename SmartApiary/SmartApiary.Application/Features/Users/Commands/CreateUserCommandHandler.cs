using MediatR;
using SmartApiary.Application.Interfaces.Repositories;

namespace SmartApiary.Application.Features.Users.Commands
{
    public class CreateUserCommandHandler(IUserRepository userRepository)
        : IRequestHandler<CreateUserCommand, Guid>
    {
        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken ct)
        {
            var activationToken = Guid.NewGuid().ToString();
            var expiresAtUtc = DateTime.UtcNow.AddDays(1);

            var user = SmartApiary.Domain.Models.User.Create(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Role,
                activationToken,
                expiresAtUtc
            );

            await userRepository.AddAsync(user, ct);
            await userRepository.SaveChangesAsync(ct);

            return user.Id;
        }
    }
}