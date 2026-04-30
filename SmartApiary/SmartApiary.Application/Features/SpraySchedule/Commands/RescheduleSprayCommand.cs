using MediatR;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SpraySchedule.Commands
{
    public record RescheduleSprayCommand(
        Guid Id,
        string StartDateTime
    ) : IRequest<Result>;
}
