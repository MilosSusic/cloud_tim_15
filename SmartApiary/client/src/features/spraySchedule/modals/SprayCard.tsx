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
}

const SprayCard: React.FC<SprayCardProps> = ({ spray, onCancel, onComplete, onReschedule }) => {
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
    alert(`
Spray Details:
- ID: ${spray.id}
- Field ID: ${spray.fieldId}
- Pesticide: ${spray.pesticideType ?? "N/A"}
- Start: ${new Date(spray.startDateTime).toLocaleString()}
- Duration: ${spray.durationHours}h
- Status: ${sprayStatusNames[spray.status]}
    `);
  };

  return (
    <div className="spray-card" onClick={handleCardClick}>
      <div className="spray-info">
        <div><strong>Start:</strong> {new Date(spray.startDateTime).toLocaleString()}</div>
        <div><strong>Duration:</strong> {spray.durationHours}h</div>
        <div><strong>Status:</strong> {sprayStatusNames[spray.status]}</div>
      </div>

      <div className="spray-actions">
        <button className="action-btn secondary" onClick={(e) => { e.stopPropagation(); onCancel(spray.id); }} disabled={spray.status === SprayStatus.Completed}>
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
