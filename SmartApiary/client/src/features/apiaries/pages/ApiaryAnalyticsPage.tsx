import React, { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
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
import { getApiaryAnalytics, saveYieldRecord } from "../../scales/api/scaleApi";

export const ApiaryAnalyticsPage: React.FC = () => {
  const navigate = useNavigate();
  const { connection } = useSignalR();
  const [apiaries, setApiaries] = useState<Apiary[]>([]);
  const [activeApiaryId, setActiveApiaryId] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const [latestTelemetry, setLatestTelemetry] = useState<TelemetryData | null>(
    null,
  );
  const [yieldChartData, setYieldChartData] = useState<DailyYieldData[]>([]);
  const [climateChartData, setClimateChartData] = useState<ClimateData[]>([]);

  const measurementBufferRef = useRef<Map<string, { weight: number; timestamp: string }[]>>(new Map());
  const periodCounterRef = useRef<Map<string, number>>(new Map());
  const blockCounterRef = useRef<Map<string, number>>(new Map());

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

    measurementBufferRef.current.clear();
    periodCounterRef.current.clear();
    blockCounterRef.current.clear();
    setError(null);
  }, [activeApiaryId]);

  useEffect(() => {
    if (!connection || !activeApiaryId) return;

    if (connection.state !== "Connected") {
      console.log("[SignalR] Waiting for connection to be established...");
      return;
    }

    measurementBufferRef.current.clear();
    periodCounterRef.current.clear();
    blockCounterRef.current.clear();

    connection
      .invoke("JoinApiaryGroup", activeApiaryId)
      .then(() => console.log(`Prijavljen na grupu: ${activeApiaryId}`))
      .catch((err) => console.error("Greška pri prijavi na grupu:", err));

    connection.on("ReceiveScaleTelemetry", (data: any) => {
      console.log("Stigao novi podatak sa vage preko SignalR-a:", data);

      const scaleId = data.scaleId;
      
      const newTelemetry: TelemetryData = {
        weight: data.weight,
        temperature: data.temperature,
        humidity: data.humidity,
        batteryLevel: data.batteryLevel,
        timestamp: data.timestamp,
      };

      setLatestTelemetry(newTelemetry);

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

      // Inicijalizuj buffer za ovu vagu ako ne postoji
      if (!measurementBufferRef.current.has(scaleId)) {
        measurementBufferRef.current.set(scaleId, []);
        periodCounterRef.current.set(scaleId, 0);
        blockCounterRef.current.set(scaleId, 0);
      }

      const buffer = measurementBufferRef.current.get(scaleId)!;
      buffer.push({
        weight: data.weight,
        timestamp: data.timestamp,
      });

      console.log(
        `[BUFFER] Scale ${scaleId}: Measurement added. Current count: ${buffer.length}/12`,
      );

      if (buffer.length === 12) {
        const blockCounter = blockCounterRef.current.get(scaleId)!;
        blockCounterRef.current.set(scaleId, blockCounter + 1);
        const newBlockCounter = blockCounterRef.current.get(scaleId)!;

        if (newBlockCounter % 2 === 1) {
          const firstWeight = buffer[0].weight;
          const lastWeight = buffer[11].weight;
          const delta = Number((lastWeight - firstWeight).toFixed(2));

          const periodCounter = periodCounterRef.current.get(scaleId)!;
          periodCounterRef.current.set(scaleId, periodCounter + 1);
          const newPeriodCounter = periodCounterRef.current.get(scaleId)!;

          const firstTimestamp = new Date(buffer[0].timestamp);
          const endTimestamp = new Date(buffer[11].timestamp);
          const timeLabel = endTimestamp.toLocaleTimeString("sr-RS", {
            hour: "2-digit",
            minute: "2-digit",
            second: "2-digit",
          });

          const durationMs = endTimestamp.getTime() - firstTimestamp.getTime();
          const durationSec = durationMs / 1000;

          console.log(
            `[YIELD] Scale ${scaleId} Period ${newPeriodCounter}: ${firstWeight}kg → ${lastWeight}kg = ${delta}kg`,
          );
          console.log(`[YIELD] Time range: ${firstTimestamp.toLocaleTimeString("sr-RS", {
            hour: "2-digit",
            minute: "2-digit",
            second: "2-digit",
          })} → ${timeLabel}`);
          console.log(`[YIELD] Duration of 12 measurements: ${durationSec} seconds`);

          setYieldChartData((prev) => [...prev, { date: timeLabel, delta }].slice(-10));

          saveYieldRecord({
            scaleId: scaleId,
            deltaKg: delta,
            recordedAt: endTimestamp.toISOString(),
          }).catch((err) => console.error("[YIELD RECORD] Failed to save:", err));
        } else {
          console.log(
            `[BUFFER] Scale ${scaleId}: Block ${newBlockCounter} skipped (night period)`,
          );
        }

        measurementBufferRef.current.set(scaleId, []);
        console.log(`[BUFFER] Scale ${scaleId}: Buffer reset. Ready for next period.`);
      }
    });

    return () => {
      if (connection && connection.state === "Connected") {
        connection.off("ReceiveScaleTelemetry");
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

        // Učitaj historiju iz Table Storage-a
        const analytics = await getApiaryAnalytics(activeApiaryId, 1);

        console.log("[INIT] Full analytics response:", analytics);

        // Postavi yield historiju (zadnjih 10)
        const yieldData = (analytics.yieldLog || analytics.yieldHistory || []).slice(-10);
        console.log("[INIT] Yield data after slice:", yieldData);
        setYieldChartData(yieldData);

        // Postavi climate historiju (zadnjih 10)
        const climateData = (analytics.climateLog || analytics.climateHistory || []).slice(-10);
        console.log("[INIT] Climate data after slice:", climateData);
        setClimateChartData(climateData);

        // Postavi latest telemetry ako postoji
        if (analytics.latestTelemetry) {
          console.log("[INIT] Latest telemetry:", analytics.latestTelemetry);
          setLatestTelemetry(analytics.latestTelemetry);
        }

        console.log(`[INIT] Loaded ${yieldData.length} yield periods and ${climateData.length} climate measurements`);
      } catch (err: any) {
        console.error("Greška pri učitavanju historije:", err);
        console.error("[INIT] Error details:", err.message);
        setYieldChartData([]);
        setClimateChartData([]);
      }
    };

    loadInitialData();
  }, [activeApiaryId]);

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
          <button
            type="button"
            className="analytics-back-button"
            onClick={() => navigate(-1)}
            aria-label="Go back"
          >
            ← Back
          </button>
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
