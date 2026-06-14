using System.Text.Json;
using SmartApiary.ScaleSimulator.Models;
using SmartApiary.ScaleSimulator.Services;

namespace SmartApiary.ScaleSimulator
{
    internal class Program
    {
        private const string StateFileNameTemplate = "simulation-state-{0}.json";

        static async Task Main(string[] args)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("        SMART APIARY - IOT SCALE SIMULATOR         ");
            Console.WriteLine("==================================================");

            Console.Write("[INPUT] Enter Scale Serial Number (format SA-YYYY-XXXXX): ");
            string serialNumber = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(serialNumber))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] Serial number cannot be empty. Exiting...");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            using var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:7253/") };
            var deviceClient = new DeviceClient(httpClient);

            Console.WriteLine($"\n[CHECK] Contacting Azure Function for S/N: {serialNumber}...");

            // KORAK 1: Šaljemo upit sa Guid.Empty. Još uvek NE GENERIŠEMO hardverski ID!
            string? accessToken = await deviceClient.ActivateDeviceAsync(serialNumber, Guid.Empty);

            if (accessToken == "HARDWARE_ID_REQUIRED")
            {
                // SCENARIO 1: Server je potvrdio da vaga nije uparena.
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[NOT PAIRED] Scale is registered but not paired. Initiating activation protocol...");
                Console.ResetColor();

                // TEK SADA generišemo Hardware ID pošto znamo da je neophodan!
                Guid hardwareId = Guid.NewGuid();
                Console.WriteLine($"[HARDWARE] Generated Device UUID for Activation: {hardwareId}");

                Console.WriteLine("[HANDSHAKE] Sending Hardware ID to complete pairing...");
                // KORAK 2: Ponovni zahtev sa pravim hardverskim ID-jem
                accessToken = await deviceClient.ActivateDeviceAsync(serialNumber, hardwareId);

                if (!string.IsNullOrEmpty(accessToken))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[SUCCESS] Handshake successful! Device is now PAIRED.");
                    Console.ResetColor();
                }
            }
            else if (!string.IsNullOrEmpty(accessToken))
            {
                // SCENARIO 2: Server je vratio postojeći token, preskačemo generisanje ID-ja!
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[BYPASS] Device already paired. Existing access token loaded successfully!");
                Console.ResetColor();
            }

            // Krajnja provera validnosti tokena
            if (string.IsNullOrEmpty(accessToken) || accessToken == "HARDWARE_ID_REQUIRED")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] Failed to obtain Access Token. Activation rejected by server.");
                Console.ResetColor();
                Console.WriteLine("\nPress ENTER to close the simulator.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"[STORAGE] Secure Device Access Token: {accessToken}");
            Console.WriteLine("[SIMULATION] Real-time telemetry mode enabled.");
            Console.WriteLine("[SIMULATION] Measurements use current real time.");
            Console.WriteLine("[SIMULATION] Battery drops continuously and resets at 2.5%.");
            Console.WriteLine("[SIMULATION] Virtual state is persisted between restarts.\n");

            var simulatorService = new ScaleSimulatorService();
            var telemetryPublisher = new TelemetryPublisher(httpClient);

            const int delayMs = 5000;
            const double batteryResetThreshold = 2.5;
            const double initialBattery = 100.0;
            const double batteryDropPerMeasurement = 7.5;

            var stateFilePath = Path.Combine(AppContext.BaseDirectory, string.Format(StateFileNameTemplate, serialNumber));
            var state = LoadState(stateFilePath, initialBattery);

            double battery = Math.Clamp(state.Battery, 0, initialBattery);

            SaveState(stateFilePath, battery);

            while (true)
            {
                var currentTime = DateTime.UtcNow;
                var telemetryData = simulatorService.GenerateTelemetry(battery);
                telemetryData.Timestamp = currentTime;

                var result = await telemetryPublisher.PublishAsync(telemetryData, accessToken);

                if (result.Success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(
                        $"[{DateTime.Now:HH:mm:ss}] Telemetry Published -> Real Time: {currentTime:dd.MM.yyyy HH:mm:ss} | Weight: {telemetryData.Weight}kg | Temp: {telemetryData.Temperature}°C | Battery: {telemetryData.BatteryLevel}%");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Failed to publish: {result.ErrorMessage}");
                    Console.ResetColor();
                }

                if (battery <= batteryResetThreshold)
                {
                    battery = initialBattery;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[BATTERY] Battery reset to 100% at {currentTime:dd.MM.yyyy HH:mm:ss}");
                    Console.ResetColor();
                }
                else
                {
                    battery = Math.Max(batteryResetThreshold, Math.Round(battery - batteryDropPerMeasurement, 1));
                }

                SaveState(stateFilePath, battery);

                await Task.Delay(delayMs);
            }
        }

        private static SimulationState LoadState(string stateFilePath, double fallbackBattery)
        {
            try
            {
                if (!File.Exists(stateFilePath))
                {
                    return new SimulationState
                    {
                        SimulatedLocalTime = DateTime.UtcNow,
                        Battery = fallbackBattery
                    };
                }

                var json = File.ReadAllText(stateFilePath);
                var state = JsonSerializer.Deserialize<SimulationState>(json);
                return state ?? new SimulationState
                {
                    SimulatedLocalTime = DateTime.UtcNow,
                    Battery = fallbackBattery
                };
            }
            catch
            {
                return new SimulationState
                {
                    SimulatedLocalTime = DateTime.UtcNow,
                    Battery = fallbackBattery
                };
            }
        }

        private static void SaveState(string stateFilePath, double battery)
        {
            var state = new SimulationState
            {
                SimulatedLocalTime = DateTime.UtcNow,
                Battery = battery
            };

            var json = JsonSerializer.Serialize(state);
            File.WriteAllText(stateFilePath, json);
        }
    }
}