using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.ScaleSimulator.Models
{
    public class ActivationRequestDTO
    {
        public string SerialNumber { get; set; } = string.Empty;
        public Guid HardwareId { get; set; }
    }
}
