import type {
  ClimateData,
  DailyYieldData,
} from "../../apiaries/types/analytics";

export interface ApiaryAnalyticsResponse {
  latestTelemetry: {
    weight: number;
    temperature: number;
    humidity: number;
    batteryLevel: number;
    timestamp: string;
  } | null;
  yieldHistory: DailyYieldData[];
  climateHistory: ClimateData[];
  yieldLog?: DailyYieldData[];
  climateLog?: ClimateData[];
}
