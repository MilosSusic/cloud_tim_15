using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Services;
using SmartApiary.Domain.Enums;

namespace SmartApiary.Functions.Processing
{
    internal class WeatherCheckTimerFunction(
        ILogger<WeatherCheckTimerFunction> logger,
        ISprayScheduleRepository sprayScheduleRepository,
        IFieldRepository fieldRepository,
        IWeatherService weatherService)
    {
        [Function(nameof(WeatherCheckTimerFunction))]
        public async Task RunAsync(
    [TimerTrigger("0 */3 * * * *")] TimerInfo timerInfo,
    CancellationToken ct)
        {
            logger.LogInformation("WeatherCheck Timer triggered at {Time}", DateTime.UtcNow);

            // prelaz u Completed za prosla prskanja
            var pastSchedules = await sprayScheduleRepository.GetPastScheduledAsync(ct);

            foreach (var spray in pastSchedules)
            {
                spray.UpdateStatus(SprayStatus.Completed);
                sprayScheduleRepository.Update(spray);
                logger.LogInformation("Spray {SprayId} automatically marked as Completed.", spray.Id);
            }

            await sprayScheduleRepository.SaveChangesAsync(ct);

            // provjera vremena za sutrasnja prskanja
            var tomorrowSchedules = await sprayScheduleRepository.GetScheduledForTomorrowAsync(ct);

            if (tomorrowSchedules.Count == 0)
            {
                logger.LogInformation("No spray schedules found for tomorrow.");
                return;
            }

            logger.LogInformation("Found {Count} spray schedules for tomorrow.", tomorrowSchedules.Count);

            foreach (var spray in tomorrowSchedules)
            {
                var field = await fieldRepository.GetByIdAsync(spray.FieldId, ct);
                if (field is null)
                {
                    logger.LogWarning("Field {FieldId} not found for spray {SprayId}.", spray.FieldId, spray.Id);
                    continue;
                }

                var weather = await weatherService.GetForecastAsync(
                    field.Latitude, field.Longitude, spray.StartDateTime);

                if (weather.WindSpeed > 5 || weather.HasRain)
                {
                    spray.SetWeatherWarning(
                        $"⚠️ Bad weather expected – Wind: {weather.WindSpeed:F1} m/s, {weather.Description}");
                    logger.LogInformation("Bad weather warning set for spray {SprayId}.", spray.Id);
                }
                else
                {
                    spray.SetWeatherWarning(null);
                    logger.LogInformation("Weather is fine for spray {SprayId}.", spray.Id);
                }

                sprayScheduleRepository.Update(spray);
            }

            await sprayScheduleRepository.SaveChangesAsync(ct);
            logger.LogInformation("WeatherCheck Timer completed.");
        }
    }
}