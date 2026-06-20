import React from "react";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ReferenceLine,
  ResponsiveContainer,
  Cell,
} from "recharts";
import type { DailyYieldData } from "../../types/analytics";

interface NectarYieldChartProps {
  data: DailyYieldData[];
}

export const NectarYieldChart: React.FC<NectarYieldChartProps> = ({ data }) => {
  const CustomTooltip = ({ active, payload }: any) => {
    if (active && payload && payload.length) {
      const value = payload[0].value;
      const isPositive = value >= 0;
      return (
        <div
          style={{
            background: "rgba(15, 23, 42, 0.9)",
            border: "1px solid rgba(255, 255, 255, 0.1)",
            padding: "10px",
            borderRadius: "8px",
            color: "#fff",
          }}
        >
          <p style={{ margin: 0, fontSize: "0.85rem", color: "#94a3b8" }}>
            {payload[0].payload.date}
          </p>
          <p
            style={{
              margin: "5px 0 0 0",
              fontWeight: "bold",
              color: isPositive ? "#10b981" : "#ef4444",
            }}
          >
            {isPositive
              ? `Prinos: +${value.toFixed(2)} kg`
              : `Potrošnja: ${value.toFixed(2)} kg`}
          </p>
        </div>
      );
    }
    return null;
  };

  return (
    <div className="analytics-card" style={{ height: "400px" }}>
      <h3
        style={{ margin: "0 0 1.5rem 0", fontSize: "1.2rem", fontWeight: 600 }}
      >
        Daily nectar yield
      </h3>

      <ResponsiveContainer width="100%" height="90%">
        <BarChart
          data={data}
          margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
        >
          <CartesianGrid
            strokeDasharray="3 3"
            stroke="rgba(255, 255, 255, 0.05)"
          />

          <XAxis dataKey="date" stroke="#64748b" style={{ fontSize: "12px" }} />
          <YAxis stroke="#64748b" unit=" kg" style={{ fontSize: "12px" }} />

          <Tooltip content={<CustomTooltip />} />

          <ReferenceLine y={0} stroke="rgba(255, 255, 255, 0.2)" />

          <Bar dataKey="delta">
            {data.map((entry, index) => {
              const isPositive = entry.delta >= 0;
              return (
                <Cell
                  key={`cell-${index}`}
                  fill={isPositive ? "#10b981" : "#ef4444"}
                  fillOpacity={0.85}
                />
              );
            })}
          </Bar>
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
};
