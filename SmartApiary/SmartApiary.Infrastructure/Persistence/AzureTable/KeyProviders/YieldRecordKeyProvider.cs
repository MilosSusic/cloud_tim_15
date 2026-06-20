using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;
using SmartApiary.Infrastructure.Persistence.AzureTable.Common;

namespace SmartApiary.Infrastructure.Persistence.AzureTable.KeyProviders
{
    internal class YieldRecordKeyProvider : ITableKeyProvider<YieldRecord>
    {
        public string GetPartitionKey(YieldRecord entity)
        {
            return entity.SmartScaleId.ToString();
        }

        public string GetRowKey(YieldRecord entity)
        {
            return entity.RecordedAt.ToUniversalTime().Ticks.ToString("D19");
        }
    }
}
