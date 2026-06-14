using MediatR;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Services;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.SpraySchedule.Commands
{
    public class ChangeSprayStatusCommandHandler : IRequestHandler<ChangeSprayStatusCommand, Result>
    {
        private readonly ISprayScheduleRepository _sprayScheduleRepository;
        private readonly ISprayNotificationQueueService _sprayNotificationQueueService;
        private readonly ISprayLogRepository _sprayLogRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly IWeatherService _weatherService;

        public ChangeSprayStatusCommandHandler(
            ISprayScheduleRepository sprayScheduleRepository,
            ISprayNotificationQueueService sprayNotificationQueueService,
            ISprayLogRepository sprayLogRepository,
            IFieldRepository fieldRepository,
            IWeatherService weatherService)
        {
            _sprayScheduleRepository = sprayScheduleRepository;
            _sprayNotificationQueueService = sprayNotificationQueueService;
            _sprayLogRepository = sprayLogRepository;
            _fieldRepository = fieldRepository;
            _weatherService = weatherService;
        }

        public async Task<Result> Handle(ChangeSprayStatusCommand request, CancellationToken cancellationToken)
        {
            var spraySchedule = await _sprayScheduleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (spraySchedule is null)
                return Result.Failure("Spray schedule not found.");

            spraySchedule.UpdateStatus(request.NewStatus);

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

            if (request.NewStatus == SprayStatus.Completed)
            {
                var field = await _fieldRepository.GetByIdAsync(
                    spraySchedule.FieldId, cancellationToken);

                var weather = await _weatherService.GetCurrentWeatherAsync(
                    field!.Latitude, field.Longitude);

                var activeCrop = field?.Crops?.FirstOrDefault(c => !c.IsDeleted);

                var sprayLogResult = SprayLog.Create(
                    spraySchedule.Id,
                    spraySchedule.FieldId,
                    field?.FieldName ?? string.Empty,
                    activeCrop?.CropName.ToString(),
                    spraySchedule.StartDateTime,
                    spraySchedule.StartDateTime.AddHours(spraySchedule.DurationHours),
                    spraySchedule.PesticideType,
                    weather.WindSpeed,
                    weather.Temperature,
                    weather.Description);

                if (sprayLogResult.IsFailure)
                    return Result.Failure(sprayLogResult.Error!);

                var sprayLog = sprayLogResult.Value!;

                await _sprayLogRepository.AddAsync(sprayLog, cancellationToken);
                await _sprayLogRepository.SaveChangesAsync(cancellationToken);
            }

            return Result.Success();
        }
    }
}