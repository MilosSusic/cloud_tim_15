using SmartApiary.ScaleSimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.ScaleSimulator.Services
{
    public class ScaleSimulatorService
    {
        private readonly Random _random = new();

        public TelemetryDTO GenerateTelemetry(double currentBattery)
        {
            return new TelemetryDTO
            {
                Weight = Math.Round(45.0 + (_random.NextDouble() * 3 - 1.5), 2),
                Humidity = Math.Round(50.0 + _random.Next(-10, 10), 1),
                Temperature = Math.Round(33.5 + (_random.NextDouble() * 2), 1),
                BatteryLevel = Math.Round(currentBattery, 1)
            };
        }
    }
}
