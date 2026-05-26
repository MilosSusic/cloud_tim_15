using SmartApiary.Domain.Common;
using SmartApiary.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Domain.Models
{
    public class Telemetry
    {
        public EntityId Id { get; private set; }
        public EntityId SmartScaleId { get; private set; }
        public Weight Weight { get; private set; }
        public Percentage Humidity { get; private set; }
        public TemperatureValue Temperature { get; private set; }
        public Percentage BatteryLevel { get; private set; }
        public DateTime Timestamp { get; private set; }

        // Privatni konstruktor - koristi se isključivo unutar fabrike
        private Telemetry(
            EntityId id,
            EntityId smartScaleId,
            Weight weight,
            Percentage humidity,
            TemperatureValue temperature,
            Percentage batteryLevel,
            DateTime timestamp)
        {
            Id = id;
            SmartScaleId = smartScaleId;
            Weight = weight;
            Humidity = humidity;
            Temperature = temperature;
            BatteryLevel = batteryLevel;
            Timestamp = timestamp;
        }

        #region Factory Methods

        // CREATE: Koristi se kada stiže POTPUNO NOVO merenje sa simulatora (generiše novi ID)
        public static Result<Telemetry> Create(
            string smartScaleId,
            double weight,
            double humidity,
            double temperature,
            double batteryLevel,
            DateTime timestamp)
        {
            if (timestamp > DateTime.UtcNow)
                return Result<Telemetry>.Failure("Measurement timestamp cannot be in the future.");

            // 1. Validacija i kreiranje ID-ja vage
            var scaleIdResult = EntityId.Create(smartScaleId);
            if (scaleIdResult.IsFailure)
                return Result<Telemetry>.Failure(scaleIdResult.Error!);

            // 2. Validacija i kreiranje Težine
            var weightResult = Weight.Create(weight);
            if (weightResult.IsFailure)
                return Result<Telemetry>.Failure(weightResult.Error!);

            // 3. Validacija i kreiranje Vlažnosti (procenat)
            var humidityResult = Percentage.Create(humidity); // Ili metoda koju tvoj procenat koristi, npr. FromRaw
            if (humidityResult.IsFailure)
                return Result<Telemetry>.Failure(humidityResult.Error!);

            // 4. Validacija i kreiranje Temperature
            var temperatureResult = TemperatureValue.Create(temperature);
            if (temperatureResult.IsFailure)
                return Result<Telemetry>.Failure(temperatureResult.Error!);

            // 5. Validacija i kreiranje Baterije (procenat)
            var batteryResult = Percentage.Create(batteryLevel);
            if (batteryResult.IsFailure)
                return Result<Telemetry>.Failure(batteryResult.Error!);

            // Vraćamo uspeh sa novim ID-jem za ovo merenje
            return Result<Telemetry>.Success(new Telemetry(
                EntityId.New(),
                scaleIdResult.Value,
                weightResult.Value,
                humidityResult.Value,
                temperatureResult.Value,
                batteryResult.Value,
                timestamp
            ));
        }

        // LOAD: Koristi se isključivo u Mapper-u kada se podaci čitaju iz Azure Table Storage-a
        public static Result<Telemetry> Load(
            string id,
            string smartScaleId,
            double weight,
            double humidity,
            double temperature,
            double batteryLevel,
            DateTime timestamp)
        {
            var idResult = EntityId.Create(id);
            if (idResult.IsFailure)
                return Result<Telemetry>.Failure(idResult.Error!);

            var scaleIdResult = EntityId.Create(smartScaleId);
            if (scaleIdResult.IsFailure)
                return Result<Telemetry>.Failure(scaleIdResult.Error!);

            var weightResult = Weight.Create(weight);
            if (weightResult.IsFailure)
                return Result<Telemetry>.Failure(weightResult.Error!);

            var humidityResult = Percentage.Create(humidity);
            if (humidityResult.IsFailure)
                return Result<Telemetry>.Failure(humidityResult.Error!);

            var temperatureResult = TemperatureValue.Create(temperature);
            if (temperatureResult.IsFailure)
                return Result<Telemetry>.Failure(temperatureResult.Error!);

            var batteryResult = Percentage.Create(batteryLevel);
            if (batteryResult.IsFailure)
                return Result<Telemetry>.Failure(batteryResult.Error!);

            var measurement = new Telemetry(
                idResult.Value,
                scaleIdResult.Value,
                weightResult.Value,
                humidityResult.Value,
                temperatureResult.Value,
                batteryResult.Value,
                timestamp
            );

            return Result<Telemetry>.Success(measurement);
        }

        #endregion
    }
}
