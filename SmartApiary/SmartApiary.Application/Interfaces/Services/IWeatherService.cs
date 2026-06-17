using SmartApiary.Application.Common;

namespace SmartApiary.Application.Interfaces.Services
{
    public interface IWeatherService
    {
        Task<WeatherInfo> GetCurrentWeatherAsync(double lat, double lon);

        Task<WeatherInfo> GetForecastAsync(double lat, double lon, DateTime dateTime);
    }
}