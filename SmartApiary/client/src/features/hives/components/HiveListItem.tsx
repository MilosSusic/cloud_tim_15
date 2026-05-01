import React from "react";
import type { Hive } from "../types/Hive";

interface Props {
  hive: Hive;
  onEdit: (hive: Hive) => void;
  onDelete: (hive: Hive) => void;
  onOpenLog: (hive: Hive) => void;
}

const HiveListItem: React.FC<Props> = ({ hive, onEdit, onDelete, onOpenLog }) => (
  <li className="hives-list-item">
    <div className="hives-list-main">
      <div className="hives-list-label">{hive.label}</div>
      <div className="hives-list-meta">{hive.type} · {hive.color} · queen {hive.queenAge}y</div>
      {hive.note && <div className="hives-list-note">{hive.note}</div>}
    </div>
    <div className="hives-list-actions">
      <button type="button" className="hives-icon-btn" onClick={() => onOpenLog(hive)} aria-label="Open hive log" title="Log">📓</button>
      <button type="button" className="hives-icon-btn" onClick={() => onEdit(hive)} aria-label="Edit hive" title="Edit">✏️</button>
      <button type="button" className="hives-icon-btn hives-icon-btn-danger" onClick={() => onDelete(hive)} aria-label="Delete hive" title="Delete">🗑️</button>
    </div>
  </li>
);

export default HiveListItem;
