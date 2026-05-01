using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Features.Fields.Commands;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Application.Interfaces.Repositories;
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
    public class CreateSprayScheduleCommandHandler : IRequestHandler<CreateSprayScheduleCommand, Result<Guid>>
    {
        private readonly ISprayScheduleRepository _sprayScheduleRepository;
        private readonly ISprayNotificationQueueService _sprayNotificationQueueService;

        public CreateSprayScheduleCommandHandler(
            ISprayScheduleRepository sprayScheduleRepository,
            ISprayNotificationQueueService sprayNotificationQueueService)
        {
            _sprayScheduleRepository = sprayScheduleRepository;
            _sprayNotificationQueueService = sprayNotificationQueueService;
        }

        public async Task<Result<Guid>> Handle(CreateSprayScheduleCommand request, CancellationToken cancellationToken)
        {
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

            await _sprayNotificationQueueService.SendAsync(
                new SprayNotificationMessage(
                    spraySchedule.Id,
                    spraySchedule.FieldId,
                    "Scheduled",
                    spraySchedule.StartDateTime,
                    spraySchedule.DurationHours,
                    spraySchedule.PesticideType),
                cancellationToken);

            return Result<Guid>.Success(spraySchedule.Id);
        }
    }
}