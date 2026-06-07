using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Common.Options;
using SmartApiary.Infrastructure.Persistence.AzureTable.Common;
using SmartApiary.Infrastructure.Persistence.AzureTable.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Persistence.AzureTable.Repositories
{
    internal class TelemetryRepository : ITelemetryRepository
    {
        private readonly TableClient _tableClient;
        private readonly ITableKeyProvider<Telemetry> _keyProvider;
        private readonly ITableMapper<Telemetry, TelemetryEntity> _mapper;

        //public TelemetryRepository(
        //    TableServiceClient tableServiceClient,
        //    ITableKeyProvider<Telemetry> keyProvider,
        //    ITableMapper<Telemetry, TelemetryEntity> mapper,
        //    IOptions<AzureTableOptions> options)
        //{
        //    _keyProvider = keyProvider;
        //    _mapper = mapper;

        //    _tableClient = tableServiceClient.GetTableClient(options.Value.ScaleMeasurementsTable);
        //}
        public TelemetryRepository(
        string connectionString,
        string tableName,
        ITableKeyProvider<Telemetry> keyProvider,
        ITableMapper<Telemetry, TelemetryEntity> mapper)
        {
            _keyProvider = keyProvider;
            _mapper = mapper;

            // Više nema if provera, odmah pravimo klijenta
            _tableClient = new TableClient(connectionString, tableName);
            _tableClient.CreateIfNotExists();
        }

        public async Task SaveAsync(Telemetry telemetry, CancellationToken ct)
        {
            TelemetryEntity entity = _mapper.ToEntity(telemetry);

            entity.PartitionKey = _keyProvider.GetPartitionKey(telemetry);
            entity.RowKey = _keyProvider.GetRowKey(telemetry);
                
            await _tableClient.AddEntityAsync(entity, cancellationToken: ct);
        }

        public async Task<Telemetry?> GetLatestBeforeAsync(string scaleId, DateTimeOffset timestamp, CancellationToken ct)
        {
            string partitionKey = scaleId;

            // Posto je tabela sortirana opadajuce (najnoviji na vrhu), starije merenje ima veci RowKey
            string currentInvertedTicks = (DateTime.MaxValue.Ticks - timestamp.Ticks).ToString("d19");

            string filter = TableClient.CreateQueryFilter($"PartitionKey eq {partitionKey} and RowKey gt {currentInvertedTicks}");

            var queryPage = _tableClient.QueryAsync<TelemetryEntity>(filter, maxPerPage: 1, cancellationToken: ct);

            await foreach (var entity in queryPage.WithCancellation(ct))
            {
                if (entity != null)
                {
                    return _mapper.ToDomain(entity);
                }
            }

            return null; 
        }
    }
}
