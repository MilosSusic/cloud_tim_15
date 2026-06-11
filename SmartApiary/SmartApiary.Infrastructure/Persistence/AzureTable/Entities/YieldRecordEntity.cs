using Azure;
using Azure.Data.Tables;

namespace SmartApiary.Infrastructure.Persistence.AzureTable.Entities
{
    public class YieldRecordEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string Id { get; set; } = string.Empty;
        public string SmartScaleId { get; set; } = string.Empty;
        public double DeltaKg { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
