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

        public async Task<Telemetry?> GetLatestBeforeAsync(Telemetry current, CancellationToken ct)
        {
            string partitionKey = _keyProvider.GetPartitionKey(current);
            string filter = TableClient.CreateQueryFilter(
                $"PartitionKey eq {partitionKey} and ObservationTime lt {current.Timestamp:yyyy-MM-ddTHH:mm:ssZ}");

            var queryPage = _tableClient.QueryAsync<TelemetryEntity>(filter, maxPerPage: 1, cancellationToken: ct);

            await foreach (var entity in queryPage.WithCancellation(ct))
            {
                if (entity == null)
                {
                    continue;
                }

                return _mapper.ToDomain(entity);
            }

            return null;
        }

        public async Task<IEnumerable<Telemetry>> GetHistoryAsync(string scaleId, DateTime startFrom, CancellationToken ct = default)
        {
            var telemetryList = new List<Telemetry>();

            // Filtriramo po particiji (vaga) i vremenu (ObservationTime vece ili jednako od trazenog startnog datuma)
            string filter = TableClient.CreateQueryFilter($"PartitionKey eq {scaleId} and ObservationTime ge {startFrom:yyyy-MM-ddTHH:mm:ssZ}");

            var queryPage = _tableClient.QueryAsync<TelemetryEntity>(filter, cancellationToken: ct);

            await foreach (var entity in queryPage.WithCancellation(ct))
            {
                if (entity != null)
                {
                    var domainModel = _mapper.ToDomain(entity);
                    if (domainModel != null)
                    {
                        telemetryList.Add(domainModel);
                    }
                }
            }

            // Vracamo sortirano hronoloski (od najstarijeg ka najnovijem) da bi grafik lakse crtao s leva na desno
            return telemetryList.OrderBy(t => t.Timestamp);
        }
    }
}
