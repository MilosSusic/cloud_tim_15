using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Persistence.AzureTable.Entities
{
    internal class TelemetryEntity : BaseTableEntity
    {
        public double Weight { get; set; }
        public double Humidity { get; set; }
        public double Temperature { get; set; }
        public double BatteryLevel { get; set; }
        public DateTime ObservationTime { get; set; }
    }
}
