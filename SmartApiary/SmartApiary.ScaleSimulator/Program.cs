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
            Console.WriteLine("       SMART APIARY - IOT SCALE SIMULATOR         ");
            Console.WriteLine("==================================================");

            string serialNumber = "";

            // 1. Uhvati parametar ako je aplikacija pokrenuta automatski preko Handlera
            if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            {
                serialNumber = args[0];
                Console.WriteLine($"[AUTO-START] Scale triggered for S/N: {serialNumber}");
            }
            else
            {
                // Ako je pokreneš ručno (F5), pitaće te da uneseš serijski broj
                Console.Write("[INPUT] Enter Scale Serial Number (format SA-YYYY-XXXXX): ");
                serialNumber = Console.ReadLine()?.Trim() ?? "";
            }

            if (string.IsNullOrWhiteSpace(serialNumber))
            {
                Console.WriteLine("[ERROR] Serial number cannot be empty. Exiting...");
                Console.ReadLine();
                return;
            }

            // 2. Generisanje jedinstvenog 128-bitnog UUID (Guid) identifikatora hardvera
            Guid hardwareId = Guid.NewGuid();
            Console.WriteLine($"[HARDWARE] Generated Device UUID: {hardwareId}");

            // 3. Podešavanje HTTP klijenta za lokalnu Azure funkciju (port 7253)
            using var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:7253/") };
            var deviceClient = new DeviceClient(httpClient);

            Console.WriteLine("\n[HANDSHAKE] Sending unique identifier and serial number to Azure Function...");

            // 4. Slanje podataka ka Azure funkciji (Handshake)
            string? accessToken = await deviceClient.ActivateDeviceAsync(serialNumber, hardwareId);

            // 5. Provera odgovora
            if (string.IsNullOrEmpty(accessToken))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[HANDSHAKE FAILED] Azure Function rejected activation. Check logs.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[SUCCESS] Handshake successful! Device is now PAIRED.");
                Console.ResetColor();
                Console.WriteLine($"[STORAGE] Secure Device Access Token: {accessToken}");
            }

            Console.WriteLine("\n==================================================");
            Console.WriteLine("Press ENTER to close the simulator.");
            Console.WriteLine("==================================================");
            Console.ReadLine();
        }
    }
}