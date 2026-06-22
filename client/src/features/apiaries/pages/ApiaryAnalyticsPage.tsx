import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useSignalR } from "../../../shared/context/SignalRContext";
import { useAuth } from "../../../shared/auth/AuthContext";
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

interface ExtendedTelemetryData extends TelemetryData {
  scaleId?: string;
}

export const ApiaryAnalyticsPage: React.FC = () => {
  const navigate = useNavigate();
  const { connection } = useSignalR();
  const { logout } = useAuth();
  const [apiaries, setApiaries] = useState<Apiary[]>([]);
  const [activeApiaryId, setActiveApiaryId] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const [latestTelemetry, setLatestTelemetry] =
    useState<ExtendedTelemetryData | null>(null);
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
    setError(null);
  }, [activeApiaryId]);

  useEffect(() => {
    if (!connection || !activeApiaryId) return;

    if (connection.state !== "Connected") {
      console.log("[SignalR] Waiting for connection to be established...");
      return;
    }

    connection
      .invoke("JoinApiaryGroup", activeApiaryId)
      .then(() => console.log(`Prijavljen na grupu: ${activeApiaryId}`))
      .catch((err) => console.error("Greška pri prijavi na grupu:", err));

    connection.on("ReceiveScaleTelemetry", (data: any) => {
      console.log("Stigao novi podatak sa vage preko SignalR-a:", data);

      const scaleId = data.scaleId || data.smartScaleId;
      setLatestTelemetry({
        weight: data.weight,
        temperature: data.temperature,
        humidity: data.humidity,
        batteryLevel: data.batteryLevel,
        timestamp: data.timestamp,
        scaleId: scaleId,
      });

      const dateObj = new Date(data.timestamp);
      const timeStr = dateObj.toLocaleTimeString("sr-RS", {
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
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
      );
    });
    // LISTENER 2: Slusanje gotovih delti sa BEKENDA za Gornji Grafik (Prinos)
    connection.on("ReceiveScaleYield", (data: any) => {
      console.log("[PRINOS UŽIVO] Backend izračunao novu deltu:", data);

      const timeLabel = new Date(data.timestamp).toLocaleTimeString("sr-RS", {
        hour: "2-digit",
        minute: "2-digit",
      });

      setYieldChartData((prev) =>
        [...prev, { date: timeLabel, delta: data.delta }].slice(-10),
      );
    });

    return () => {
      if (connection && connection.state === "Connected") {
        connection.off("ReceiveScaleTelemetry");
        connection.off("ReceiveScaleYield");
        connection
          .invoke("LeaveApiaryGroup", activeApiaryId)
          .then(() => console.log(`Odjavljen sa grupe: ${activeApiaryId}`))
          .catch((err) => console.error("Greška pri odjavi sa grupe:", err));
      }
    };
  }, [activeApiaryId, connection]);

  useEffect(() => {
    if (!activeApiaryId) return;

    const loadInitialData = async () => {
      try {
        setError(null);

        // Ucitaj istoriju iz Table Storage-a
        const analytics = await getApiaryAnalytics(activeApiaryId, 1);

        console.log("[INIT] Full analytics response:", analytics);

        // Postavi yield istoriju (zadnjih 10)
        const yieldData = (
          analytics.yieldLog ||
          analytics.yieldHistory ||
          []
        ).slice(-10);
        console.log("[INIT] Yield data after slice:", yieldData);
        setYieldChartData(yieldData);

        // Postavi climate istoriju (zadnjih 10)
        const climateData = (
          analytics.climateLog ||
          analytics.climateHistory ||
          []
        ).slice(-10);
        console.log("[INIT] Climate data after slice:", climateData);
        setClimateChartData(climateData);

        if (analytics.latestTelemetry) {
          setLatestTelemetry({
            weight: analytics.latestTelemetry.weight,
            temperature: analytics.latestTelemetry.temperature,
            humidity: analytics.latestTelemetry.humidity,
            batteryLevel: analytics.latestTelemetry.batteryLevel,
            timestamp: analytics.latestTelemetry.timestamp,
            scaleId:
              analytics.latestTelemetry.scaleId ||
              analytics.latestTelemetry.smartScaleId,
          });
        } else {
          setLatestTelemetry(null);
        }

        console.log(
          `[INIT] Loaded ${yieldData.length} yield periods and ${climateData.length} climate measurements`,
        );
      } catch (err: any) {
        console.error("Greška pri učitavanju historije:", err);
        console.error("[INIT] Error details:", err.message);
        setYieldChartData([]);
        setClimateChartData([]);
        setLatestTelemetry(null);
      }
    };

    loadInitialData();
  }, [activeApiaryId]);

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  if (loading) {
    return <div className="analytics-page-container">Loading analytics...</div>;
  }

  return (
    <div className="analytics-page-container">
      <div className="analytics-header">
        <div className="analytics-header-top">
          <h2 className="analytics-title">
            Smart Apiary
            <span className="analytics-title-separator"> / </span>
            <span className="analytics-title-accent">Analytics</span>
          </h2>
          <div style={{ display: "flex", gap: "0.75rem" }}>
            <button
              type="button"
              className="analytics-back-button"
              onClick={() => navigate(-1)}
              aria-label="Go back"
            >
              ← Back
            </button>
            <button
              type="button"
              className="analytics-back-button"
              style={{
                background: "rgba(255,80,80,0.1)",
                border: "1px solid rgba(255,80,80,0.35)",
                color: "#ff6b6b",
              }}
              onClick={handleLogout}
            >
              ⎋ Logout
            </button>
          </div>
        </div>
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
