using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Services;
using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace SmartApiary.Infrastructure.Services
{
    public class ScaleSimulatorLauncher : IScaleSimulatorLauncher
    {
        private readonly ILogger<ScaleSimulatorLauncher> _logger;

        public ScaleSimulatorLauncher(ILogger<ScaleSimulatorLauncher> logger)
        {
            _logger = logger;
        }

        public void Launch(SmartScale scale)
        {
            try
            {
                // Start from the current bin directory wherever the application is running
                string currentDir = AppContext.BaseDirectory;
                string simulatorPath = null;

                // Traverse up through the folders until the simulator project directory is found
                while (currentDir != null)
                {
                    var potentialPath = Path.Combine(currentDir, "SmartApiary.ScaleSimulator", "bin", "Debug", "net8.0", "SmartApiary.ScaleSimulator.exe");
                    if (File.Exists(potentialPath))
                    {
                        simulatorPath = potentialPath;
                        break;
                    }
                    currentDir = Directory.GetParent(currentDir)?.FullName;
                }

                if (simulatorPath == null || !File.Exists(simulatorPath))
                {
                    _logger.LogError("[SIMULATOR ERROR] Simulator .exe file could not be found in the folder structure!");
                    return;
                }

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = simulatorPath,
                    Arguments = $"\"{scale.SerialNumber}\" \"{scale.AccessToken}\"",
                    CreateNoWindow = false,
                    UseShellExecute = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
                };

                System.Diagnostics.Process.Start(startInfo);
                _logger.LogInformation($"[SIMULATOR] Successfully started process for scale: {scale.SerialNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while starting simulator for {scale.SerialNumber}: {ex.Message}");
            }
        }
    }
}
