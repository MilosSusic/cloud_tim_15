import React from "react";
import type { Apiary } from "../types/Apiary";
import type { NearbyField } from "../types/NearbyField";
import HivesSection from "../../hives/components/HivesSection";

interface Props {
  selectedApiary: Apiary | null;
  selectedField: NearbyField | null;
  onClose: () => void;
  onEdit: (apiary: Apiary) => void;
  onMessage: (msg: string) => void;
}

const ApiaryInfoPanel: React.FC<Props> = ({ selectedApiary, selectedField, onClose, onEdit, onMessage }) => (
  <aside className="apiary-info-panel">
    <div className="apiary-info-header">
      <h3>{selectedApiary ? "Apiary Info" : selectedField ? "Field Info" : "Info"}</h3>
      {(selectedApiary || selectedField) && (
        <button className="apiary-info-close" onClick={onClose} aria-label="Close info panel">×</button>
      )}
    </div>

    {!selectedApiary && !selectedField ? (
      <div className="apiary-info-empty">Select an apiary or field from the map to see details.</div>
    ) : selectedApiary ? (
      <>
        {(selectedApiary.thumbnailUrl || selectedApiary.imageUrl) && (
          <img
            className="apiary-info-image"
            src={selectedApiary.thumbnailUrl || selectedApiary.imageUrl}
            alt={selectedApiary.name}
          />
        )}
        <div className="apiary-info-section">
          <div className="apiary-info-row"><span>Name</span><strong>{selectedApiary.name}</strong></div>
          <div className="apiary-info-row"><span>Latitude</span><strong>{selectedApiary.latitude}</strong></div>
          <div className="apiary-info-row"><span>Longitude</span><strong>{selectedApiary.longitude}</strong></div>
          {selectedApiary.description && (
            <div className="apiary-info-row apiary-info-row-block">
              <span>Description</span>
              <p>{selectedApiary.description}</p>
            </div>
          )}
        </div>
        <button type="button" className="apiary-info-edit-btn" onClick={() => onEdit(selectedApiary)}>
          Edit Apiary
        </button>
        <HivesSection apiaryId={selectedApiary.id} onSuccess={onMessage} onError={onMessage} />
      </>
    ) : selectedField ? (
      <div className="field-info-panel" style={{ padding: 16 }}>
        <div style={{ fontWeight: "bold", fontSize: 18, marginBottom: 8 }}>{selectedField.fieldName}</div>
        <div style={{ marginBottom: 6 }}><span style={{ fontWeight: "bold" }}>Owner:</span> {selectedField.owner.firstName} {selectedField.owner.lastName}</div>
        <div style={{ marginBottom: 6 }}><span style={{ fontWeight: "bold" }}>Email:</span> {selectedField.owner.email}</div>
        <div style={{ marginBottom: 6 }}><span style={{ fontWeight: "bold" }}>Phone:</span> {selectedField.owner.phone}</div>
        <div style={{ marginTop: 10 }}>
          <span style={{ fontWeight: "bold" }}>Crops:</span>
          {selectedField.crops.length === 0 ? (
            <span style={{ marginLeft: 6, color: "#888" }}>No crops</span>
          ) : (
            <ul style={{ margin: 0, paddingLeft: 18 }}>
              {selectedField.crops.map((crop, idx) => (
                <li key={idx} style={{ display: "flex", alignItems: "center", marginBottom: 2 }}>
                  {crop.icon && <img src={crop.icon} alt={crop.name} style={{ width: 16, height: 16, marginRight: 4 }} />}
                  <span style={{ color: "#228B22", fontWeight: 500 }}>{crop.name}</span>
                  <span style={{ marginLeft: 4, fontSize: 12, color: "#666" }}>({crop.floweringPeriod})</span>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    ) : null}
  </aside>
);

export default ApiaryInfoPanel;
