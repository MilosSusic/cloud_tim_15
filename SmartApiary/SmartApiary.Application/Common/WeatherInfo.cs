namespace SmartApiary.Application.Common
{
    public class WeatherInfo
    {
        public double WindSpeed { get; set; }
        public double Temperature { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool HasRain { get; set; }
    }
}
