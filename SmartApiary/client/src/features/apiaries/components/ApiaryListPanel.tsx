import React from "react";
import type { Apiary } from "../types/Apiary";

interface Props {
  apiaries: Apiary[];
  selectedId?: string | null;
  loading: boolean;
  onSelect: (apiary: Apiary) => void;
}

const ApiaryListPanel: React.FC<Props> = ({ apiaries, selectedId, loading, onSelect }) => (
  <aside className="apiary-list-card">
    <div className="apiary-list-card-header">
      <h3>Apiaries</h3>
      <span className="apiary-list-count-badge">{apiaries.length}</span>
    </div>
    <ul>
      {!loading && apiaries.length === 0 && (
        <div className="apiary-list-empty">No apiaries.</div>
      )}
      {apiaries.map((apiary) => (
        <li
          key={apiary.id}
          className={selectedId === apiary.id ? "active" : ""}
          onClick={() => onSelect(apiary)}
        >
          <span className="apiary-list-icon">🐝</span>
          <div className="apiary-list-body">
            <div className="apiary-list-name">{apiary.name}</div>
            <div className="apiary-list-desc">{apiary.description}</div>
            <div className="apiary-list-coords">
              {Number(apiary.latitude).toFixed(3)}, {Number(apiary.longitude).toFixed(3)}
            </div>
          </div>
        </li>
      ))}
    </ul>
  </aside>
);

export default ApiaryListPanel;
