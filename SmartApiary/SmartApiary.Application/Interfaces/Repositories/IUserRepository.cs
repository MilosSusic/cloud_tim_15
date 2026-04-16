using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user, CancellationToken ct = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<User?> GetByActivationTokenAsync(string activationToken, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
        Task<User?> GetByResetPasswordTokenAsync(string resetPasswordToken, CancellationToken ct = default);
    }
}