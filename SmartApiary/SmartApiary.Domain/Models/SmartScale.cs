using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using System;
using System.Text.RegularExpressions;

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
        public double? WeightDropLimit { get; private set; }

        private static readonly Regex SerialNumberRegex = new(@"^SA-\d{4}-\d{5}$", RegexOptions.Compiled);

        public static Result<SmartScale> Create(Guid hiveId, string serialNumber, double? weightDropLimit)
        {
            var validation = ValidateCreate(hiveId, serialNumber, weightDropLimit);
            if (validation.IsFailure)
                return Result<SmartScale>.Failure(validation.Error!);

            return Result<SmartScale>.Success(new SmartScale
            {
                Id = Guid.NewGuid(),
                HiveId = hiveId,
                SerialNumber = serialNumber,
                Status = SmartScaleStatus.Unpaired,
                WeightDropLimit = weightDropLimit
            });
        }

        public Result Activate(Guid hardwareId, string accessToken)
        {
            var validation = ValidateActivation(hardwareId, accessToken);
            if (validation.IsFailure)
                return validation;

            HardwareId = hardwareId;
            AccessToken = accessToken;
            Status = SmartScaleStatus.Paired;

            return Result.Success();
        }

        private static Result ValidateCreate(Guid hiveId, string serialNumber, double? weightDropLimit)
        {
            if (hiveId == Guid.Empty)
                return Result.Failure("Hive id is required.");

            if (string.IsNullOrWhiteSpace(serialNumber) || !SerialNumberRegex.IsMatch(serialNumber))
                return Result.Failure("Serial number must be in format SA-YYYY-XXXXX.");

            if (weightDropLimit.HasValue && weightDropLimit.Value <= 0)
                return Result.Failure("Weight drop limit must be greater than zero.");

            return Result.Success();
        }

        private Result ValidateActivation(Guid hardwareId, string accessToken)
        {
            if (Status == SmartScaleStatus.Paired)
                return Result.Failure("Device is already paired.");

            if (hardwareId == Guid.Empty)
                return Result.Failure("Hardware id (UUID) is required.");

            if (string.IsNullOrWhiteSpace(accessToken))
                return Result.Failure("Access token is required.");

            return Result.Success();
        }
    }
}
