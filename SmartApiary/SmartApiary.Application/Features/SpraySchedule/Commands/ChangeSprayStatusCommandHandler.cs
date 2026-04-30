using MediatR;
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

        public ChangeSprayStatusCommandHandler(ISprayScheduleRepository sprayScheduleRepository)
        {
            _sprayScheduleRepository = sprayScheduleRepository;
        }

        public async Task<Result> Handle(ChangeSprayStatusCommand request, CancellationToken cancellationToken)
        {
            var spraySchedule = await _sprayScheduleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (spraySchedule is null)
                return Result.Failure("Spray schedule not found.");

            spraySchedule.Status = request.NewStatus;
            spraySchedule.UpdatedAt = DateTime.Now;

            _sprayScheduleRepository.Update(spraySchedule);
            await _sprayScheduleRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

    }
}
