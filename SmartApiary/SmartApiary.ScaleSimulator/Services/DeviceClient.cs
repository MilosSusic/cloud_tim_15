using SmartApiary.ScaleSimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.ScaleSimulator.Services
{
    public class DeviceClient
    {
        private readonly HttpClient _httpClient;

        public DeviceClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string?> ActivateDeviceAsync(string serialNumber, Guid hardwareId)
        {
            var payload = new ActivationRequestDTO
            {
                SerialNumber = serialNumber,
                HardwareId = hardwareId
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/devices/activate", payload);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadFromJsonAsync<ApiResponse<ActivationResult>>();
                    return responseBody?.Data?.AccessToken;
                }

                return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }

}
