using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace SmartApiary.Infrastructure.Services
{
    public class StartupScaleSimulator : IStartupScaleSimulator
    {
        private readonly ISmartScaleRepository _scaleRepository;
        private readonly IApiaryRepository _apiaryRepository;
        private readonly IHiveRepository _hiveRepository;
        private readonly IScaleSimulatorLauncher _simulatorLauncher;
        private readonly ILogger<StartupScaleSimulator> _logger;

        public StartupScaleSimulator(
            ISmartScaleRepository scaleRepository,
            IApiaryRepository apiaryRepository,
            IHiveRepository hiveRepository,
            IScaleSimulatorLauncher simulatorLauncher,
            ILogger<StartupScaleSimulator> logger)
        {
            _scaleRepository = scaleRepository;
            _apiaryRepository = apiaryRepository;
            _hiveRepository = hiveRepository;
            _simulatorLauncher = simulatorLauncher;
            _logger = logger;
        }

        public async Task WarmUpScalesForUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[STARTUP SIMULATOR] Checking scales to initialize for user: {userId}");

            try
            {
                // 1. Povuci sve pčelinjake koji pripadaju ulogovanom korisniku
                var userApiaries = await _apiaryRepository.GetByOwnerIdAsync(userId, cancellationToken);

                foreach (var apiary in userApiaries)
                {
                    // 2. Za svaki pčelinjak povuci njegove košnice
                    var hives = await _hiveRepository.GetByApiaryIdAsync(apiary.Id, cancellationToken);

                    foreach (var hive in hives)
                    {
                        // 3. Proveri da li ta košnica ima registrovanu pametnu vagu
                        var scale = await _scaleRepository.GetByHiveIdAsync(hive.Id, cancellationToken);

                        if (scale != null)
                        {
                            _logger.LogInformation($"[STARTUP SIMULATOR] Launching simulator for scale: {scale.SerialNumber}");

                            // Pozivamo tvoj launcher koji izvršava .exe fajl sa parametrima
                            _simulatorLauncher.Launch(scale);
                        }
                    }
                }

                _logger.LogInformation($"[STARTUP SIMULATOR] Warm-up sequence completed for user: {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[STARTUP SIMULATOR ERROR] Error during scale warmup for user {userId}: {ex.Message}");
            }
        }
    }
}
