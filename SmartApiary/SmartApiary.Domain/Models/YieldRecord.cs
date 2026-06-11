using SmartApiary.Domain.Common;
using SmartApiary.Domain.ValueObjects;
using System;

namespace SmartApiary.Domain.Models
{
    /// <summary>
    /// Predstavlja zabilježen prinos (razlika težine izme?u dva mjerenja).
    /// Sprema se u Table Storage za brze pretraživanja bez potrebe da se ra?una iz Telemetrije.
    /// </summary>
    public class YieldRecord
    {
        public EntityId Id { get; private set; }
        public EntityId SmartScaleId { get; private set; }
        public double Delta { get; private set; }
        public DateTime RecordedAt { get; private set; }

        private YieldRecord(
            EntityId id,
            EntityId smartScaleId,
            double delta,
            DateTime recordedAt)
        {
            Id = id;
            SmartScaleId = smartScaleId;
            Delta = delta;
            RecordedAt = recordedAt;
        }

        #region Factory Methods

        /// <summary>
        /// Kreira novi YieldRecord kada se završi period od 12 mjerenja.
        /// Koristi se na frontu da se proslje?uje backendi-u.
        /// </summary>
        public static Result<YieldRecord> Create(
            string smartScaleId,
            double deltaKg,
            DateTime recordedAt)
        {
            if (recordedAt > DateTime.UtcNow)
                return Result<YieldRecord>.Failure("Record timestamp cannot be in the future.");

            var scaleIdResult = EntityId.Create(smartScaleId);
            if (scaleIdResult.IsFailure)
                return Result<YieldRecord>.Failure(scaleIdResult.Error!);

            return Result<YieldRecord>.Success(new YieldRecord(
                EntityId.New(),
                scaleIdResult.Value,
                deltaKg,
                recordedAt
            ));
        }

        /// <summary>
        /// U?itava YieldRecord iz Table Storage-a.
        /// Koristi se u Mapper-u.
        /// </summary>
        public static Result<YieldRecord> Load(
            string id,
            string smartScaleId,
            double deltaKg,
            DateTime recordedAt)
        {
            var idResult = EntityId.Create(id);
            if (idResult.IsFailure)
                return Result<YieldRecord>.Failure(idResult.Error!);

            var scaleIdResult = EntityId.Create(smartScaleId);
            if (scaleIdResult.IsFailure)
                return Result<YieldRecord>.Failure(scaleIdResult.Error!);

            return Result<YieldRecord>.Success(new YieldRecord(
                idResult.Value,
                scaleIdResult.Value,
                deltaKg,
                recordedAt
            ));
        }

        #endregion
    }
}
