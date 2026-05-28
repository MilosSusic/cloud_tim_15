using SmartApiary.ScaleSimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.ScaleSimulator.Services
{
    public class TelemetryPublisher(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        public async Task<(bool Success, string? ErrorMessage)> PublishAsync(TelemetryDTO telemetry, string deviceToken)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, "api/scales/telemetry");
                request.Content = JsonContent.Create(telemetry);

                request.Headers.Add("X-Device-Token", deviceToken);

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                return (false, $"API returned status code: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
