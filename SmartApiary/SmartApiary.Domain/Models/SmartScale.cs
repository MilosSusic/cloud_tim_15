using SmartApiary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartApiary.Domain.Models
{
    public class SmartScale
    {
        public Guid Id { get; private set; }
        public Guid HiveId { get; private set; }
        public string SerialNumber { get; private set; } = string.Empty;
        public Guid? HardwareId { get; private set; }
        public string? AccessToken { get; private set; }
        public SmartScaleStatus Status { get; private set; }

        private static readonly Regex SerialNumberRegex = new(@"^SA-\d{4}-\d{5}$", RegexOptions.Compiled);

        public static SmartScale Create(Guid hiveId, string serialNumber)
        {
            if (hiveId == Guid.Empty)
                throw new ArgumentException("Hive id is required.", nameof(hiveId));

            if (string.IsNullOrWhiteSpace(serialNumber) || !SerialNumberRegex.IsMatch(serialNumber))
                throw new ArgumentException("Serial number must be in format SA-YYYY-XXXXX.", nameof(serialNumber));

            return new SmartScale
            {
                Id = Guid.NewGuid(),
                HiveId = hiveId,
                SerialNumber = serialNumber,
                Status = SmartScaleStatus.Unpaired
            };
        }

        public void Activate(Guid hardwareId, string accessToken)
        {
            if (Status == SmartScaleStatus.Paired)
                throw new InvalidOperationException("Device is already paired.");

            if (hardwareId == Guid.Empty)
                throw new ArgumentException("Hardware id (UUID) is required.", nameof(hardwareId));

            if (string.IsNullOrWhiteSpace(accessToken))
                throw new ArgumentException("Access token is required.", nameof(accessToken));

            HardwareId = hardwareId;
            AccessToken = accessToken;
            Status = SmartScaleStatus.Paired;
        }
    }
}
