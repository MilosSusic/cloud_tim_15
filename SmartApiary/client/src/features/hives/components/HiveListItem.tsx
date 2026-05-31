import React from "react";
import type { Hive } from "../types/Hive";

interface Props {
  hive: Hive;
  onEdit: (hive: Hive) => void;
  onDelete: (hive: Hive) => void;
  onOpenLog: (hive: Hive) => void;
  onOpenScale: (hive: Hive) => void;
}

const HiveListItem: React.FC<Props> = ({
  hive,
  onEdit,
  onDelete,
  onOpenLog,
  onOpenScale,
}) => (
  <li className="hives-list-item">
    <div className="hives-list-main">
      {/* <div className="hives-list-label">{hive.label}</div> */}
      <div style={{ display: "flex", alignItems: "center", gap: "8px" }}>
        <div className="hives-list-label">{hive.label}</div>
        {hive.scaleSerialNumber && (
          <span
            style={{
              fontSize: "11px",
              background: "rgba(34, 197, 94, 0.15)",
              color: "#4ade80",
              padding: "2px 6px",
              borderRadius: "6px",
              fontWeight: 600,
            }}
          >
            ⚖️ {hive.scaleSerialNumber}
          </span>
        )}
      </div>
      <div className="hives-list-meta">
        {hive.type} · {hive.color} · queen {hive.queenAge}y
      </div>
      {hive.note && <div className="hives-list-note">{hive.note}</div>}
    </div>
    <div className="hives-list-actions">
      {!hive.scaleSerialNumber && (
        <button
          type="button"
          className="hives-icon-btn"
          onClick={() => onOpenScale(hive)}
          aria-label="Add scale"
          title="Add Scale"
          style={{ color: "#fbbf24" }}
        >
          ⚖️
        </button>
      )}
      <button
        type="button"
        className="hives-icon-btn"
        onClick={() => onOpenLog(hive)}
        aria-label="Open hive log"
        title="Log"
      >
        📓
      </button>
      <button
        type="button"
        className="hives-icon-btn"
        onClick={() => onEdit(hive)}
        aria-label="Edit hive"
        title="Edit"
      >
        ✏️
      </button>
      <button
        type="button"
        className="hives-icon-btn hives-icon-btn-danger"
        onClick={() => onDelete(hive)}
        aria-label="Delete hive"
        title="Delete"
      >
        🗑️
      </button>
    </div>
  </li>
);

export default HiveListItem;
