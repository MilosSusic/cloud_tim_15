import React from "react";
import type { Apiary } from "../../types/Apiary";

interface AnalyticsTabsProps {
  apiaries: Apiary[];
  activeApiaryId: string | null;
  onTabChange: (apiaryId: string) => void;
}

export const AnalyticsTabs: React.FC<AnalyticsTabsProps> = ({
  apiaries,
  activeApiaryId,
  onTabChange,
}) => {
  return (
    <div className="analytics-tabs-container">
      {apiaries.map((apiary) => (
        <button
          key={apiary.id}
          className={`analytics-tab-button ${activeApiaryId === apiary.id ? "active" : ""}`}
          onClick={() => onTabChange(apiary.id)}
        >
          {apiary.name}
        </button>
      ))}
    </div>
  );
};
