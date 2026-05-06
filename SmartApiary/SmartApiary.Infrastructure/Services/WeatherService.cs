using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SmartApiary.Application.Interfaces.Services;

namespace SmartApiary.Infrastructure.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenWeather:ApiKey"]!;
        }

        public async Task<WeatherInfo> GetCurrentWeatherAsync(double lat, double lon)
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_apiKey}&units=metric";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);

            return new WeatherInfo
            {
                WindSpeed = json.RootElement
                    .GetProperty("wind").GetProperty("speed").GetDouble(),
                Temperature = json.RootElement
                    .GetProperty("main").GetProperty("temp").GetDouble(),
                Description = json.RootElement
                    .GetProperty("weather")[0]
                    .GetProperty("description").GetString() ?? ""
            };
        }

        public async Task<WeatherInfo> GetForecastAsync(double lat, double lon, DateTime dateTime)
        {
            var url = $"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&appid={_apiKey}&units=metric";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);

            var forecasts = json.RootElement.GetProperty("list");
            JsonElement? closest = null;
            long minDiff = long.MaxValue;
            var targetUnix = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();

            foreach (var item in forecasts.EnumerateArray())
            {
                var dt = item.GetProperty("dt").GetInt64();
                var diff = Math.Abs(dt - targetUnix);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closest = item;
                }
            }

            if (closest is null)
                return new WeatherInfo();

            var hasRain = closest.Value.TryGetProperty("rain", out _);
            var weatherMain = closest.Value
                .GetProperty("weather")[0]
                .GetProperty("main").GetString() ?? "";

            return new WeatherInfo
            {
                WindSpeed = closest.Value
                    .GetProperty("wind").GetProperty("speed").GetDouble(),
                Temperature = closest.Value
                    .GetProperty("main").GetProperty("temp").GetDouble(),
                Description = closest.Value
                    .GetProperty("weather")[0]
                    .GetProperty("description").GetString() ?? "",
                HasRain = hasRain || weatherMain == "Rain"
            };
        }
    }
}