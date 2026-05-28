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

            string serialNumber = "";
            string accessToken = "";

            // 1. Hvatanje parametara prosleđenih iz Launchera
            if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            {
                serialNumber = args[0].Trim('"');
                Console.WriteLine($"[AUTO-START] Scale triggered for S/N: {serialNumber}");

                if (args.Length > 1 && !string.IsNullOrWhiteSpace(args[1]))
                {
                    accessToken = args[1].Trim('"');
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("[AUTO-START] Existing Access Token detected. Skipping Handshake!");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("[AUTO-START] No token received from launcher. Will attempt activation.");
                }
            }
            else
            {
                // Ručno pokretanje (F5 u simulatoru)
                Console.Write("[INPUT] Enter Scale Serial Number (format SA-YYYY-XXXXX): ");
                serialNumber = Console.ReadLine()?.Trim() ?? "";
            }

            // Validacija serijskog broja
            if (string.IsNullOrWhiteSpace(serialNumber))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] Serial number cannot be empty. Exiting...");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            // Inicijalizacija HTTP klijenta i klijenta za uređaj (Mora biti pre bilo kakve provere!)
            using var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:7253/") };
            var deviceClient = new DeviceClient(httpClient);

            // 2. Provera: Ako nemamo token, tek tada generišemo Hardware ID i radimo Handshake
            if (string.IsNullOrEmpty(accessToken))
            {
                // 🚀 Hardware ID se pravi i ispisuje SAMO ako se radi prva aktivacija
                Guid hardwareId = Guid.NewGuid();
                Console.WriteLine($"[HARDWARE] New Device UUID Generated for Activation: {hardwareId}");

                Console.WriteLine("\n[HANDSHAKE] Sending unique identifier and serial number to Azure Function...");
                accessToken = await deviceClient.ActivateDeviceAsync(serialNumber, hardwareId);

                if (string.IsNullOrEmpty(accessToken))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[HANDSHAKE FAILED] Azure Function rejected activation. Check logs.");
                    Console.ResetColor();
                    Console.WriteLine("\nPress ENTER to close the simulator.");
                    Console.ReadLine();
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[SUCCESS] Handshake successful! Device is now PAIRED via API.");
                Console.ResetColor();
            }
            else
            {
                // Ako token postoji, samo ispisujemo BYPASS i idemo dalje
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[BYPASS] Device safely initialized using startup token configuration.");
                Console.ResetColor();
            }

            // --- TELEMETRIJSKI MOD (Za oba režima) ---
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