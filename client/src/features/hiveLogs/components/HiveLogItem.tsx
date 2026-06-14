import React from "react";
import type { HiveLogEntryDto } from "../types/HiveLogEntry";

interface Props {
  entry: HiveLogEntryDto;
  onEdit: (entry: HiveLogEntryDto) => void;
  onDelete: (entry: HiveLogEntryDto) => void;
}

const HiveLogItem: React.FC<Props> = ({ entry, onEdit, onDelete }) => (
  <li className="hivelog-item">
    <div className="hivelog-item-head">
      <div className="hivelog-item-date">{new Date(entry.reviewDate).toLocaleString()}</div>
      <div className="hivelog-item-actions">
        <button type="button" className="hives-icon-btn" onClick={() => onEdit(entry)} title="Edit">✏️</button>
        <button type="button" className="hives-icon-btn hives-icon-btn-danger" onClick={() => onDelete(entry)} title="Delete">🗑️</button>
      </div>
    </div>
    <div className="hivelog-item-type">{entry.reviewType}</div>
    <div className="hivelog-item-stats">
      <span>Honey: <strong>{entry.honeyAmountKg} kg</strong> ({entry.honeyFrames} frames)</span>
      <span>Brood: <strong>{entry.broodFrames}</strong> frames</span>
      <span>Queen: <strong>{entry.queenPresent ? "Yes" : "No"}</strong></span>
      {entry.bottomBoardColor && <span>Bottom board: <strong>{entry.bottomBoardColor}</strong></span>}
    </div>
    {entry.note && <div className="hivelog-item-note">{entry.note}</div>}
  </li>
);

export default HiveLogItem;
