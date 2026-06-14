export interface TelemetryData {
  weight: number;
  temperature: number;
  humidity: number;
  batteryLevel: number;
  timestamp: string;
}

export interface DailyYieldData {
  date: string;
  delta: number;
}

export interface ClimateData {
  time: string;
  temperature: number;
  humidity: number;
}
