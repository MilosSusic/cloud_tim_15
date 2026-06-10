import React from "react";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from "recharts";
import type { ClimateData } from "../../types/analytics";

interface ClimateTrendChartProps {
  data: ClimateData[];
}

export const ClimateTrendChart: React.FC<ClimateTrendChartProps> = ({
  data,
}) => {
  return (
    <div className="analytics-card" style={{ height: "400px" }}>
      <h3
        style={{ margin: "0 0 1.5rem 0", fontSize: "1.2rem", fontWeight: 600 }}
      >
        Climate Trends (Temperature and Humidity)
      </h3>

      <ResponsiveContainer width="100%" height="90%">
        <LineChart
          data={data}
          margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
        >
          <CartesianGrid
            strokeDasharray="3 3"
            stroke="rgba(255, 255, 255, 0.05)"
          />

          <XAxis dataKey="time" stroke="#64748b" style={{ fontSize: "12px" }} />

          <YAxis
            yAxisId="left"
            stroke="#f59e0b"
            unit="°C"
            style={{ fontSize: "12px" }}
          />

          <YAxis
            yAxisId="right"
            orientation="right"
            stroke="#3b82f6"
            unit="%"
            style={{ fontSize: "12px" }}
          />

          <Tooltip
            contentStyle={{
              background: "rgba(15, 23, 42, 0.9)",
              border: "1px solid rgba(255, 255, 255, 0.1)",
              borderRadius: "8px",
              color: "#fff",
            }}
          />

          <Legend wrapperStyle={{ paddingTop: "10px" }} />

          <Line
            yAxisId="left"
            type="monotone"
            dataKey="temperature"
            name="Temperatura"
            stroke="#f59e0b"
            strokeWidth={3}
            dot={{ r: 2 }}
            activeDot={{ r: 6 }}
          />

          <Line
            yAxisId="right"
            type="monotone"
            dataKey="humidity"
            name="Vlažnost vazduha"
            stroke="#3b82f6"
            strokeWidth={3}
            dot={{ r: 2 }}
            activeDot={{ r: 6 }}
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
};
