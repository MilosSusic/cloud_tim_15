using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Interfaces.Services
{
    public interface IScaleSimulatorLauncher
    {
        void Launch(SmartScale scale);
    }
}
