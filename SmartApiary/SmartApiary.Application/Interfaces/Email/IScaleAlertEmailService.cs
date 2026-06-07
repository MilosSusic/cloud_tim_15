using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Interfaces.Email
{
    public interface IScaleAlertEmailService
    {
        Task SendAnomalyAlertAsync(string toEmail, string anomalyType, string message, double currentValue);
    }
}
