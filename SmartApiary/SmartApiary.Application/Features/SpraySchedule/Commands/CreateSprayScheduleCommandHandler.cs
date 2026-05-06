using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Features.Fields.Commands;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Services;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SpraySchedule.Commands
{
    public class CreateSprayScheduleCommandHandler : IRequestHandler<CreateSprayScheduleCommand, Result<CreateSprayScheduleResult>>
    {
        private readonly ISprayScheduleRepository _sprayScheduleRepository;
        private readonly ISprayNotificationQueueService _sprayNotificationQueueService;
        private readonly ISprayNotificationRepository _sprayNotificationRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly IWeatherService _weatherService;

        public CreateSprayScheduleCommandHandler(
            ISprayScheduleRepository sprayScheduleRepository,
            ISprayNotificationQueueService sprayNotificationQueueService,
            ISprayNotificationRepository sprayNotificationRepository,
            IFieldRepository fieldRepository,
            IWeatherService weatherService)
        {
            _sprayScheduleRepository = sprayScheduleRepository;
            _sprayNotificationQueueService = sprayNotificationQueueService;
            _sprayNotificationRepository = sprayNotificationRepository;
            _fieldRepository = fieldRepository;
            _weatherService = weatherService;
        }

        public async Task<Result<CreateSprayScheduleResult>> Handle(CreateSprayScheduleCommand request, CancellationToken cancellationToken)
        {
            var field = await _fieldRepository.GetByIdAsync(
            request.FieldId, cancellationToken);

            if (field is null)
                return Result<CreateSprayScheduleResult>.Failure("Field not found.");

            var weather = await _weatherService.GetForecastAsync(
                field.Latitude, field.Longitude, request.StartDateTime);

            bool badWeather = weather.WindSpeed > 5 || weather.HasRain;

            var spraySchedule = new Domain.Models.SpraySchedule
            {
                Id = Guid.NewGuid(),
                FieldId = request.FieldId,
                StartDateTime = request.StartDateTime,
                DurationHours = request.DurationHours,
                PesticideType = request.PesticideType
            };

            await _sprayScheduleRepository.AddAsync(spraySchedule, cancellationToken);
            await _sprayScheduleRepository.SaveChangesAsync(cancellationToken);

            var recipients = await _sprayNotificationRepository
                .GetFieldRecipientsInRadiusAsync(request.FieldId, 5.0, cancellationToken);

            var count = recipients?.Recipients.Count ?? 0;

            await _sprayNotificationQueueService.SendAsync(
                new SprayNotificationMessage(
                    spraySchedule.Id,
                    spraySchedule.FieldId,
                    "Scheduled",
                    spraySchedule.StartDateTime,
                    spraySchedule.DurationHours,
                    spraySchedule.PesticideType),
                cancellationToken);

            return Result<CreateSprayScheduleResult>.Success(new CreateSprayScheduleResult
            {
                SprayScheduleId = spraySchedule.Id,
                NotifiedBeekeepersCount = count,
                WeatherWarning = badWeather
                ? "Bad weather conditions – consider rescheduling."
                : null
            });
        }
    }
}