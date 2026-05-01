using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Interfaces.Services
{
    public interface ISprayNotificationOrchestrator
    {
        Task<Result> ProcessAsync(SprayNotificationMessage payload, CancellationToken ct = default);
    }
}
