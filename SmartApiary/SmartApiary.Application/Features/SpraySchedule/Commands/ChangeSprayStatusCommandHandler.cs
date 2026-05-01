using MediatR;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SpraySchedule.Commands
{
    public class ChangeSprayStatusCommandHandler : IRequestHandler<ChangeSprayStatusCommand, Result>
    {
        private readonly ISprayScheduleRepository _sprayScheduleRepository;
        private readonly ISprayNotificationQueueService _sprayNotificationQueueService;

        public ChangeSprayStatusCommandHandler(
            ISprayScheduleRepository sprayScheduleRepository,
            ISprayNotificationQueueService sprayNotificationQueueService)
        {
            _sprayScheduleRepository = sprayScheduleRepository;
            _sprayNotificationQueueService = sprayNotificationQueueService;
        }

        public async Task<Result> Handle(ChangeSprayStatusCommand request, CancellationToken cancellationToken)
        {
            var spraySchedule = await _sprayScheduleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (spraySchedule is null)
                return Result.Failure("Spray schedule not found.");

            spraySchedule.Status = request.NewStatus;
            spraySchedule.UpdatedAt = DateTime.UtcNow;

            _sprayScheduleRepository.Update(spraySchedule);
            await _sprayScheduleRepository.SaveChangesAsync(cancellationToken);

            if (request.NewStatus == SprayStatus.Cancelled)
            {
                await _sprayNotificationQueueService.SendAsync(
                    new SprayNotificationMessage(
                        spraySchedule.Id,
                        spraySchedule.FieldId,
                        "Cancelled",
                        spraySchedule.StartDateTime,
                        spraySchedule.DurationHours,
                        spraySchedule.PesticideType),
                    cancellationToken);
            }

            return Result.Success();
        }
    }
}
