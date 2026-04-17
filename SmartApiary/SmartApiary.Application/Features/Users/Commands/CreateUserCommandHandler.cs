using MediatR;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.Users.Commands
{
    public class CreateUserCommandHandler(
        IUserRepository userRepository,
        IActivationQueueService activationQueueService)
        : IRequestHandler<CreateUserCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken ct)
        {
            var existingUser = await userRepository.GetByEmailAsync(request.Email, ct);

            if (existingUser is not null)
            {
                return Result<Guid>.Failure("A user with this email already exists.");
            }

            var activationToken = Guid.NewGuid().ToString();
            var expiresAtUtc = DateTime.UtcNow.AddDays(1);

            var user = User.Create(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Role,
                activationToken,
                expiresAtUtc
            );

            await userRepository.AddAsync(user, ct);
            await userRepository.SaveChangesAsync(ct);

            await activationQueueService.SendActivationAsync(
                user.Email,
                user.ActivationToken!,
                ct);

            return Result<Guid>.Success(user.Id);
        }
    }
}