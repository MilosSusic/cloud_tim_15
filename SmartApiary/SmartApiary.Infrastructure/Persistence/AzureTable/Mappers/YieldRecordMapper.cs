using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence.AzureTable.Common;
using SmartApiary.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartApiary.Infrastructure.Persistence.AzureTable.Mappers
{
    internal class YieldRecordMapper : ITableMapper<YieldRecord, YieldRecordEntity>
    {
        public YieldRecordEntity ToEntity(YieldRecord domain)
        {
            return new YieldRecordEntity
            {
                Id = domain.Id.ToString(),
                SmartScaleId = domain.SmartScaleId.ToString(),
                DeltaKg = domain.Delta,
                RecordedAt = domain.RecordedAt,
                Timestamp = DateTimeOffset.UtcNow
            };
        }

        public YieldRecord? ToDomain(YieldRecordEntity entity)
        {
            if (entity == null)
                return null;

            var result = YieldRecord.Load(
                entity.Id,
                entity.SmartScaleId,
                entity.DeltaKg,
                entity.RecordedAt
            );

            return result.IsSuccess ? result.Value : null;
        }
    }
}
