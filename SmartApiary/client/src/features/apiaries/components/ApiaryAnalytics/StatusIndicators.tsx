import React from "react";
import type { TelemetryData } from "../../types/analytics";

interface StatusIndicatorsProps {
  latestTelemetry: TelemetryData | null;
}

export const StatusIndicators: React.FC<StatusIndicatorsProps> = ({
  latestTelemetry,
}) => {
  const formatTimestamp = (isoString: string) => {
    if (!isoString) return "Nema podataka";
    const date = new Date(isoString);
    return date.toLocaleTimeString("sr-RS", {
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
    });
  };

  if (!latestTelemetry) {
    return (
      <div className="status-indicators-container">
        <div className="analytics-card indicator-item">
          <p className="indicator-label">Scale status</p>
          <div className="indicator-subtext">Waiting for data...</div>
        </div>
      </div>
    );
  }

  return (
    <div className="status-indicators-container">
      <div className="analytics-card indicator-item weight">
        <div className="indicator-label">Current Weight</div>
        <div className="indicator-value">
          {latestTelemetry.weight.toFixed(2)} kg
        </div>
        <div className="indicator-subtext">
          Updated: {formatTimestamp(latestTelemetry.timestamp)}
        </div>
      </div>

      <div className="analytics-card indicator-item climate">
        <div className="indicator-label">Temperature / Humidity</div>
        <div className="indicator-value">
          {latestTelemetry.temperature.toFixed(1)}°C
        </div>
        <div className="indicator-subtext">
          Humidity: {latestTelemetry.humidity}%
        </div>
      </div>

      <div className="analytics-card indicator-item battery">
        <div className="indicator-label">Battery Level</div>
        <div className="indicator-value">{latestTelemetry.batteryLevel}%</div>
        <div className="indicator-subtext">
          Status:{" "}
          {latestTelemetry.batteryLevel <= 15 ? "🚨 Critical" : "🔋 Normal "}
        </div>
      </div>
    </div>
  );
};
