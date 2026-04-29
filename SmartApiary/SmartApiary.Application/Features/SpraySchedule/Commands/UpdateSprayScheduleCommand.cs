using MediatR;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SpraySchedule.Commands
{
    public record UpdateSprayScheduleCommand(
        Guid Id,
        DateTime StartDateTime,
        int DurationHours,
        string? PesticideType,
        SprayStatus Status
    ) : IRequest<Result>;
}
