using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence.AzureTable.Common;
using SmartApiary.Infrastructure.Persistence.AzureTable.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Persistence.AzureTable.Mappers
{
    internal class TelemetryTableMapper : ITableMapper<Telemetry, TelemetryEntity>
    {
        public TelemetryEntity ToEntity(Telemetry domain)
        {
            return new TelemetryEntity
            {
                PartitionKey = domain.SmartScaleId.Value,
                RowKey = $"{(DateTime.MaxValue.Ticks - domain.Timestamp.Ticks).ToString("d19")}_{domain.Id.Value}",
                Weight = domain.Weight.Value,
                Humidity = domain.Humidity.Value,
                Temperature = domain.Temperature.Value,
                BatteryLevel = domain.BatteryLevel.Value,
                ObservationTime = domain.Timestamp
            };
        }

        public Telemetry? ToDomain(TelemetryEntity entity)
        {
            var parts = entity.RowKey.Split('_');
            var id = parts.Length > 1 ? parts[1] : entity.RowKey;

            var loadResult = Telemetry.Load(
                id,
                entity.PartitionKey,
                entity.Weight,
                entity.Humidity,
                entity.Temperature,
                entity.BatteryLevel,
                entity.ObservationTime
            );

            if (loadResult.IsFailure)
            {
                return null;
            }

            return loadResult.Value;
        }
    }
}
