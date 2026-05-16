using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.SpraySchedule.Commands
{
    public class UpdateSprayScheduleCommandHandler : IRequestHandler<UpdateSprayScheduleCommand, Result>
    {
        private readonly ISprayScheduleRepository _sprayScheduleRepository;

        public UpdateSprayScheduleCommandHandler(ISprayScheduleRepository sprayScheduleRepository)
        {
            _sprayScheduleRepository = sprayScheduleRepository;
        }

        public async Task<Result> Handle(UpdateSprayScheduleCommand request, CancellationToken cancellationToken)
        {
            var spraySchedule = await _sprayScheduleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (spraySchedule is null)
                return Result.Failure("Spray schedule not found.");

            spraySchedule.UpdateDetails(request.StartDateTime,request.Status, request.DurationHours, request.PesticideType);

            _sprayScheduleRepository.Update(spraySchedule);
            await _sprayScheduleRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
