using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Features.SmartScales.Events;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Services;
using SmartApiary.Domain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SmartScales.EventHandlers
{
    public class ProcessScaleYieldHandler : INotificationHandler<TelemetryReceivedEvent>
    {
        private readonly IYieldRecordRepository _yieldRecordRepository;
        private readonly IScaleNotificationService _notificationService;
        private readonly ILogger<ProcessScaleYieldHandler> _logger;

        // Staticki thread-safe bafer koji cuva merenja za svaku vagu pojedinacno u memoriji backend-a
        private static readonly ConcurrentDictionary<string, List<(double Weight, DateTime Timestamp)>> _serverBuffers = new();
        private static readonly ConcurrentDictionary<string, int> _blockCounters = new();

        public ProcessScaleYieldHandler(
            IYieldRecordRepository yieldRecordRepository,
            IScaleNotificationService notificationService,
            ILogger<ProcessScaleYieldHandler> logger)
        {
            _yieldRecordRepository = yieldRecordRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(TelemetryReceivedEvent notification, CancellationToken ct)
        {
            var telemetry = notification.Telemetry;
            string scaleId = telemetry.SmartScaleId.ToString();
            Guid apiaryId = notification.ApiaryId;

            // Inicijalizacija bafera i brojaca ako ne postoje za ovu vagu
            _serverBuffers.TryAdd(scaleId, new List<(double Weight, DateTime Timestamp)>());
            _blockCounters.TryAdd(scaleId, 0);

            var buffer = _serverBuffers[scaleId];

            lock (buffer)
            {
                buffer.Add((telemetry.Weight.Value, telemetry.Timestamp));
            }

            _logger.LogInformation("[SERVER BUFFER] Scale {ScaleId}: Measurement added. Current count: {Count}/12", scaleId, buffer.Count);

            // Kada nakupimo 12 merenja 
            if (buffer.Count == 12)
            {
                double firstWeight, lastWeight;
                DateTime endTimestamp;

                lock (buffer)
                {
                    firstWeight = buffer[0].Weight;
                    lastWeight = buffer[11].Weight;
                    endTimestamp = buffer[11].Timestamp;
                    buffer.Clear(); // Odmah praznimo bafer za sledeci dvominutni ciklus
                }

                _blockCounters[scaleId]++;
                int currentBlock = _blockCounters[scaleId];

                // Preskacemo svaki drugi blok (simulacija nocnog perioda)
                if (currentBlock % 2 == 1)
                {
                    double delta = Math.Round(lastWeight - firstWeight, 2);

                    _logger.LogInformation("[SERVER YIELD] Scale {ScaleId}: Calculated delta {Delta}kg ({First}kg -> {Last}kg)", scaleId, delta, firstWeight, lastWeight);

                    var yieldRecord = YieldRecord.Create(scaleId, delta, endTimestamp);

                    // Trajno spasavanje u Azure Table Storage
                    await _yieldRecordRepository.SaveAsync(yieldRecord.Value, ct);
                    _logger.LogInformation("[SERVER YIELD] Saved to Table Storage for Scale: {ScaleId}", scaleId);

                    // Slanje preko SignalR-a
                    try
                    {
                        await _notificationService.SendYieldDeltaNotificationAsync(apiaryId, scaleId, delta, endTimestamp);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[SERVER YIELD] Failed to broadcast via SignalR.");
                    }
                }
                else
                {
                    _logger.LogInformation("[SERVER BUFFER] Scale {ScaleId}: Block {Block} skipped (night period)", scaleId, currentBlock);
                }
            }
        }
    }
}
