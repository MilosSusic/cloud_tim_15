using MediatR;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SmartScales.Commands
{
    public record RegisterScaleCommand(
        Guid HiveId,
        string SerialNumber
    ) : IRequest<Result<Guid>>;
}
