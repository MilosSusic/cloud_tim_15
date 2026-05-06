namespace SmartApiary.Application.Interfaces.Services
{
    public interface IWeatherService
    {
        Task<WeatherInfo> GetCurrentWeatherAsync(double lat, double lon);

        Task<WeatherInfo> GetForecastAsync(double lat, double lon, DateTime dateTime);
    }

    public class WeatherInfo
    {
        public double WindSpeed { get; set; }
        public double Temperature { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool HasRain { get; set; }
    }
}