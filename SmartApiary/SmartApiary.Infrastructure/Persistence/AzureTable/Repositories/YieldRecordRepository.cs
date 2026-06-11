using Azure.Data.Tables;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence.AzureTable.Common;
using SmartApiary.Infrastructure.Persistence.AzureTable.Entities;
using SmartApiary.Infrastructure.Persistence.AzureTable.Mappers;

namespace SmartApiary.Infrastructure.Persistence.AzureTable.Repositories
{
    internal class YieldRecordRepository : IYieldRecordRepository
    {
        private readonly TableClient _tableClient;
        private readonly ITableKeyProvider<YieldRecord> _keyProvider;
        private readonly ITableMapper<YieldRecord, YieldRecordEntity> _mapper;

        public YieldRecordRepository(
            string connectionString,
            string tableName,
            ITableKeyProvider<YieldRecord> keyProvider,
            ITableMapper<YieldRecord, YieldRecordEntity> mapper)
        {
            _keyProvider = keyProvider;
            _mapper = mapper;

            _tableClient = new TableClient(connectionString, tableName);
            _tableClient.CreateIfNotExists();
        }

        public async Task SaveAsync(YieldRecord yieldRecord, CancellationToken ct)
        {
            YieldRecordEntity entity = _mapper.ToEntity(yieldRecord);

            entity.PartitionKey = _keyProvider.GetPartitionKey(yieldRecord);
            entity.RowKey = _keyProvider.GetRowKey(yieldRecord);

            await _tableClient.AddEntityAsync(entity, cancellationToken: ct);
        }

        public async Task<IEnumerable<YieldRecord>> GetHistoryAsync(string scaleId, DateTime startFrom, CancellationToken ct = default)
        {
            var yieldRecords = new List<YieldRecord>();

            string filter = TableClient.CreateQueryFilter($"PartitionKey eq {scaleId} and Timestamp ge {new DateTimeOffset(startFrom)}");

            var queryPage = _tableClient.QueryAsync<YieldRecordEntity>(filter, cancellationToken: ct);

            await foreach (var entity in queryPage.WithCancellation(ct))
            {
                if (entity != null)
                {
                    var domainModel = _mapper.ToDomain(entity);
                    if (domainModel != null)
                    {
                        yieldRecords.Add(domainModel);
                    }
                }
            }

            return yieldRecords.OrderBy(r => r.RecordedAt);
        }
    }
}
