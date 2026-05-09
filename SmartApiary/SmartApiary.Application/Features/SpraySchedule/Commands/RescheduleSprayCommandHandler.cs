using MediatR;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using System.Globalization;

namespace SmartApiary.Application.Features.SpraySchedule.Commands
{
    public class RescheduleSprayCommandHandler : IRequestHandler<RescheduleSprayCommand, Result>
    {
        private readonly ISprayScheduleRepository _sprayScheduleRepository;
        private readonly ISprayNotificationQueueService _sprayNotificationQueueService;

        public RescheduleSprayCommandHandler(
            ISprayScheduleRepository sprayScheduleRepository,
            ISprayNotificationQueueService sprayNotificationQueueService)
        {
            _sprayScheduleRepository = sprayScheduleRepository;
            _sprayNotificationQueueService = sprayNotificationQueueService;
        }

        public async Task<Result> Handle(RescheduleSprayCommand request, CancellationToken cancellationToken)
        {
            var spraySchedule = await _sprayScheduleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (spraySchedule is null)
                return Result.Failure("Spray schedule not found.");

            var parsedDateTime = DateTime.Parse(request.StartDateTime, CultureInfo.InvariantCulture);
            
            spraySchedule.Reschedule(parsedDateTime);

            _sprayScheduleRepository.Update(spraySchedule);
            await _sprayScheduleRepository.SaveChangesAsync(cancellationToken);

            await _sprayNotificationQueueService.SendAsync(
                new SprayNotificationMessage(
                    spraySchedule.Id,
                    spraySchedule.FieldId,
                    "Rescheduled",
                    spraySchedule.StartDateTime,
                    spraySchedule.DurationHours,
                    spraySchedule.PesticideType),
                cancellationToken);

            return Result.Success();
        }
    }
}
