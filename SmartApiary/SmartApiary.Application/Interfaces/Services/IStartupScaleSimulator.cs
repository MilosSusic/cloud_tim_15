using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Interfaces.Services
{
    public interface IStartupScaleSimulator
    {
        Task WarmUpScalesForUserAsync(Guid userId, CancellationToken cancellationToken);
    }
}
