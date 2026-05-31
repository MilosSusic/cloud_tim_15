using SmartApiary.ScaleSimulator.Services;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SmartApiary.ScaleSimulator.Services;

namespace SmartApiary.ScaleSimulator
{
    internal class Program
    {
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

            // --- TELEMETRIJSKI MOD ---
            Console.WriteLine($"[STORAGE] Secure Device Access Token: {accessToken}");
            Console.WriteLine("\n[SYSTEM] Entering telemetry mode. Sending data every 8 seconds...\n");

            var simulatorService = new ScaleSimulatorService();
            var telemetryPublisher = new TelemetryPublisher(httpClient);

            double battery = 100.0;
            int delayMs = 8000;

            while (true)
            {
                var telemetryData = simulatorService.GenerateTelemetry(battery);
                battery -= 0.05;
                if (battery < 0) battery = 0;

                var result = await telemetryPublisher.PublishAsync(telemetryData, accessToken);

                if (result.Success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Telemetry Published -> Weight: {telemetryData.Weight}kg | Temp: {telemetryData.Temperature}°C | Battery: {telemetryData.BatteryLevel}%");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Failed to publish: {result.ErrorMessage}");
                    Console.ResetColor();
                }

                await Task.Delay(delayMs);
            }
        }
    }
}