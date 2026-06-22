export interface TelemetryData {
  weight: number;
  temperature: number;
  humidity: number;
  batteryLevel: number;
  timestamp: string;
  scaleId?: string; // Dodajemo opciono scaleId
  smartScaleId?: string; // Dodajemo opciono smartScaleId (da pokrijemo oba naziva sa bekenda)
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
