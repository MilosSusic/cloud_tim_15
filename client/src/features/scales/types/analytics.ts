import type {
  ClimateData,
  DailyYieldData,
  TelemetryData,
} from "../../apiaries/types/analytics";

export interface ApiaryAnalyticsResponse {
  // latestTelemetry: {
  //   weight: number;
  //   temperature: number;
  //   humidity: number;
  //   batteryLevel: number;
  //   timestamp: string;
  // } | null;
  latestTelemetry: TelemetryData | null;
  yieldHistory: DailyYieldData[];
  climateHistory: ClimateData[];
  yieldLog?: DailyYieldData[];
  climateLog?: ClimateData[];
}
