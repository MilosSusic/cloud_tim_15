import React, { useEffect, useState } from "react";
import { useSignalR } from "../../../shared/context/SignalRContext";
import { getApiaries } from "../../apiaries/api/apiaryApi";
import { AnalyticsTabs } from "../components/ApiaryAnalytics/AnalyticsTabs";
import type { Apiary } from "../types/Apiary";
import type {
  TelemetryData,
  DailyYieldData,
  ClimateData,
} from "../types/analytics";
import "../layout/ApiaryAnalyticsPage.css";
import { NectarYieldChart } from "../components/ApiaryAnalytics/NectarYieldChart";
import { StatusIndicators } from "../components/ApiaryAnalytics/StatusIndicators";
import { ClimateTrendChart } from "../components/ApiaryAnalytics/ClimateTrendChart";
import { getApiaryAnalytics } from "../../scales/api/scaleApi";

export const ApiaryAnalyticsPage: React.FC = () => {
  const { connection } = useSignalR();
  const [apiaries, setApiaries] = useState<Apiary[]>([]);
  const [activeApiaryId, setActiveApiaryId] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null); // Za prikaz eventualne greške

  const [latestTelemetry, setLatestTelemetry] = useState<TelemetryData | null>(
    null,
  );
  const [yieldChartData, setYieldChartData] = useState<DailyYieldData[]>([]);
  const [climateChartData, setClimateChartData] = useState<ClimateData[]>([]);

  useEffect(() => {
    const fetchInitialData = async () => {
      try {
        const data = await getApiaries();
        setApiaries(data);
        if (data && data.length > 0) {
          setActiveApiaryId(data[0].id);
        }
      } catch (error) {
        console.error("Greška pri učitavanju pčelinjaka:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchInitialData();
  }, []);

  useEffect(() => {
    if (!activeApiaryId) return;
    const loadAnalyticsHistory = async () => {
      try {
        // Povlacimo stvarne podatke za poslednjih 7 dana
        const analytics = await getApiaryAnalytics(activeApiaryId, 7);

        // setYieldChartData(analytics.yieldHistory || []);
        // setClimateChartData(analytics.climateHistory || []);
        // Mapiranje Yield istorije (Grafik 1)
        const mappedYield = (analytics.yieldHistory || []).map((item: any) => ({
          date: item.date || item.Date,
          delta: item.netYieldKg ?? item.NetYieldKg ?? item.delta ?? item.Delta,
        }));

        // Mapiranje Climate istorije (Grafik 2)
        const mappedClimate = (analytics.climateHistory || []).map(
          (item: any) => ({
            time: item.time || item.Time,
            temperature: item.temperature ?? item.Temperature,
            humidity: item.humidity ?? item.Humidity,
          }),
        );

        setYieldChartData(mappedYield);
        setClimateChartData(mappedClimate);

        setLatestTelemetry(analytics.latestTelemetry);
        setError(null);
      } catch (err: any) {
        console.error("Greška pri učitavanju istorije:", err);
        setError(
          "There is no historical data available for this apiary or the scale is not matched.",
        );
        setYieldChartData([]);
        setClimateChartData([]);
        setLatestTelemetry(null);
      }
    };

    loadAnalyticsHistory();
  }, [activeApiaryId]);

  // Smena tabova
  useEffect(() => {
    if (!connection || !activeApiaryId) return;

    // Prijava u sobu pcelinjaka
    connection
      .invoke("JoinApiaryGroup", activeApiaryId)
      .then(() => console.log(`Prijavljen na grupu: ${activeApiaryId}`))
      .catch((err) => console.error("Greška pri prijavi na grupu:", err));

    // Otkucaji u realnom vremenu: Slusamo dogadjaj sa bekenda
    connection.on("ReceiveScaleTelemetry", (data: any) => {
      console.log("Stigao novi podatak sa vage preko SignalR-a:", data);

      const newTelemetry: TelemetryData = {
        weight: data.weight,
        temperature: data.temperature,
        humidity: data.humidity,
        batteryLevel: data.batteryLevel,
        timestamp: data.timestamp,
      };

      // Azuriramo desne kartice istog trenutka
      setLatestTelemetry(newTelemetry);

      // Azuriramo linijski grafikon klime (dodajemo novu tacku na kraj)
      const dateObj = new Date(data.timestamp);
      const timeStr = dateObj.toLocaleTimeString("sr-RS", {
        hour: "2-digit",
        minute: "2-digit",
      });

      setClimateChartData((prev) =>
        [
          ...prev,
          {
            time: timeStr,
            temperature: data.temperature ?? data.Temperature,
            humidity: data.humidity ?? data.Humidity,
          },
        ].slice(-10),
      ); // Drzi samo poslednjih 10 merenja na grafiku da ne pukne ekran
    });

    // CLEANUP: Kada korisnik klikne na drugi tab, prvo se odjavi sa stare grupe i ugasi slušalac
    return () => {
      if (connection) {
        connection.off("ReceiveScaleTelemetry");
        connection
          .invoke("LeaveApiaryGroup", activeApiaryId)
          .then(() => console.log(`Odjavljen sa grupe: ${activeApiaryId}`))
          .catch((err) => console.error("Greška pri odjavi sa grupe:", err));
      }
    };
  }, [activeApiaryId, connection]);

  if (loading) {
    return <div className="analytics-page-container">Loading analytics...</div>;
  }

  return (
    <div className="analytics-page-container">
      <div className="analytics-header">
        <h2>Smart Apiary / Analytics</h2>
        <p>Monitor honey yield and climate conditions in real time.</p>
        {error && (
          <p style={{ color: "#ef4444", fontWeight: "bold" }}>⚠️ {error}</p>
        )}
      </div>

      <AnalyticsTabs
        apiaries={apiaries}
        activeApiaryId={activeApiaryId}
        onTabChange={setActiveApiaryId}
      />

      {/* GORNJI BLOK: Prinos nektara (75% grafik, 25% statusna kartica) */}
      <div className="analytics-row">
        <div className="analytics-main-chart">
          <NectarYieldChart data={yieldChartData} />
        </div>
        <div className="analytics-side-indicators">
          <StatusIndicators latestTelemetry={latestTelemetry} />
        </div>
      </div>

      {/* DONJI BLOK: Klima (75% grafik, 25% ponovljeni/dodatni statusi) */}
      <div className="analytics-row">
        <div className="analytics-main-chart">
          <ClimateTrendChart data={climateChartData} />
        </div>
        <div className="analytics-side-indicators">
          <div
            className="analytics-card"
            style={{
              height: "100%",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              textAlign: "center",
              color: "#94a3b8",
            }}
          >
            <div>
              <p
                style={{
                  fontSize: "1.5rem",
                  color: "#fff",
                  margin: "0 0 0.5rem 0",
                }}
              >
                📡 Live Stream
              </p>
              <p style={{ fontSize: "0.85rem", margin: 0 }}>
                Automatic chart refresh is active through the SignalR group.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
