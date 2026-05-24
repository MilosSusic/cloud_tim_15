using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.ScaleSimulator.Models
{
    public class ScaleTelemetryDTO
    {
        public string DeviceAccessToken { get; set; } = string.Empty;
        public double WeightKg { get; set; }
        public double TemperatureC { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
