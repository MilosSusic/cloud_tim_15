import React, { useState } from "react";
import "../layout/SprayCard.css";
import { SprayStatus } from "../types/SprayStatus";
import "../layout/SprayCard.css"
import type { SpraySchedule } from "../types/SpraySchedule";


interface SprayCardProps {
  spray: SpraySchedule;
  onCancel: (id: string) => void;
  onComplete: (id: string) => void;
  onReschedule: (id: string, newDate: string) => void;
  onOpenDetails: (spray: SpraySchedule) => void;
}

const SprayCard: React.FC<SprayCardProps> = ({ spray, onCancel, onComplete, onReschedule, onOpenDetails }) => {
  const [showRescheduleInput, setShowRescheduleInput] = useState(false);
  const [newDate, setNewDate] = useState("");

    const sprayStatusNames: Record<number, string> = {
    [SprayStatus.Scheduled]: "Scheduled",
    [SprayStatus.Cancelled]: "Cancelled",
    [SprayStatus.Completed]: "Completed",
    [SprayStatus.Rescheduled]: "Rescheduled",
    };


  const handleRescheduleClick = () => {
    setShowRescheduleInput(true);
  };

  const handleRescheduleSubmit = () => {
    if (newDate) {
      onReschedule(spray.id, newDate);
      setShowRescheduleInput(false);
      setNewDate("");
    }
  };

  const handleCardClick = () => {
    onOpenDetails(spray);
  };

  return (
    <div className="spray-card" onClick={handleCardClick}>
      <div className="spray-info">
        <div><strong>Start:</strong> {new Date(spray.startDateTime).toLocaleString()}</div>
        <div><strong>Duration:</strong> {spray.durationHours}h</div>
        <div><strong>Status:</strong> {sprayStatusNames[spray.status]}</div>
      </div>

      {spray.weatherWarning && (
          <div className="spray-weather-warning">
              {spray.weatherWarning}
          </div>
      )}

      <div className="spray-actions">
        <button className="action-btn secondary" onClick={(e) => { e.stopPropagation(); onCancel(spray.id); }} disabled={spray.status === SprayStatus.Completed || spray.status === SprayStatus.Cancelled}>
          Cancel
        </button>
        <button className="action-btn" onClick={(e) => { e.stopPropagation(); handleRescheduleClick(); }} disabled={spray.status === SprayStatus.Completed || spray.status === SprayStatus.Cancelled}>
          Reschedule
        </button>
        {showRescheduleInput && (
        <div className="reschedule-input" onClick={(e) => e.stopPropagation()}>
          <input
            type="datetime-local"
            value={newDate}
            onChange={(e) => setNewDate(e.target.value)}
          />
          <button className="action-btn" onClick={handleRescheduleSubmit}>
            Save
          </button>
          <button
            className="action-btn secondary"
            onClick={() => {
              setShowRescheduleInput(false);
              setNewDate("");
            }}
          >
            Back
          </button>
        </div>
        
        )}
        <button
          className="action-btn"
          onClick={(e) => { e.stopPropagation(); onComplete(spray.id); }}
          disabled={spray.status === SprayStatus.Cancelled || spray.status === SprayStatus.Completed}
        >
          Complete
        </button>
      </div>

      
    </div>
  );
};

export default SprayCard;
