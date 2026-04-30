using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SpraySchedule.Commands
{
    public class RescheduleSprayCommandHandler : IRequestHandler<RescheduleSprayCommand, Result>
    {
        private readonly ISprayScheduleRepository _sprayScheduleRepository;

        public RescheduleSprayCommandHandler(ISprayScheduleRepository sprayScheduleRepository)
        {
            _sprayScheduleRepository = sprayScheduleRepository;
        }

        public async Task<Result> Handle(RescheduleSprayCommand request, CancellationToken cancellationToken)
        {
            var spraySchedule = await _sprayScheduleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (spraySchedule is null)
                return Result.Failure("Spray schedule not found.");

            var parsedDateTime = DateTime.Parse(request.StartDateTime, CultureInfo.InvariantCulture);
            spraySchedule.StartDateTime = parsedDateTime;
            spraySchedule.Status = SprayStatus.Rescheduled;
            spraySchedule.UpdatedAt = DateTime.Now;

            _sprayScheduleRepository.Update(spraySchedule);
            await _sprayScheduleRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

    }
}
