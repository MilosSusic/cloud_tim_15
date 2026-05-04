import React from "react";
import "../layout/SprayDetails.css";
import type { SpraySchedule } from "../types/SpraySchedule";

interface SprayDetailsProps {
  spray: SpraySchedule;
  onClose: () => void;
}

const safeFormatDateTime = (dateStr?: string | null) => {
  if (!dateStr) return "N/A";
  const normalized = dateStr.replace(" ", "T");
  const d = new Date(normalized);
  return isNaN(d.getTime()) ? "N/A" : d.toLocaleString();
};

const getStatusLabel = (status: number) => {
  switch (status) {
    case 1: return "Scheduled";
    case 2: return "Cancelled";
    case 3: return "Completed";
    case 4: return "Rescheduled";
    default: return "Unknown";
  }
};

const getStatusClass = (status: number) => {
  switch (status) {
    case 1: return "status-scheduled";
    case 2: return "status-cancelled";
    case 3: return "status-completed";
    case 4: return "status-rescheduled";
    default: return "status-unknown";
  }
};

const SprayDetails: React.FC<SprayDetailsProps> = ({ spray, onClose }) => {
  return (
    <div className="spray-details-overlay">
      <div className="spray-details-card">
        <button className="close-btn" onClick={onClose}>×</button>
        <h2 className="spray-details-title">Spray Schedule Details</h2>
        <div className="spray-details-body">
          <p><strong>Pesticide:</strong> {spray.pesticideType || "N/A"}</p>
          <p><strong>Start:</strong> {safeFormatDateTime(spray.startDateTime)}</p>
          <p><strong>Duration (hours):</strong> {spray.durationHours}</p>
          <p className={`spray-status ${getStatusClass(spray.status)}`}>
            <strong>Status:</strong> {getStatusLabel(spray.status)}
          </p>
          <p><strong>Created At:</strong> {safeFormatDateTime(spray.createdAt)}</p>

          {spray.status === 3 && spray.updatedAt && (
            <p><strong>Completed At:</strong> {safeFormatDateTime(spray.updatedAt)}</p>
          )}
          {spray.status === 2 && spray.updatedAt && (
            <p><strong>Cancelled At:</strong> {safeFormatDateTime(spray.updatedAt)}</p>
          )}
          {spray.status !== 2 && spray.status !== 3 && spray.updatedAt && (
            <p><strong>Updated At:</strong> {safeFormatDateTime(spray.updatedAt)}</p>
          )}
        </div>
      </div>
    </div>
  );
};

export default SprayDetails;
