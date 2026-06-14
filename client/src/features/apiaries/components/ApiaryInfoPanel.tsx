import React from "react";
import type { Apiary } from "../types/Apiary";
import type { NearbyField } from "../types/NearbyField";
import HivesSection from "../../hives/components/HivesSection";

const isLikelyImageUrl = (value?: string) => {
  if (!value) return false;
  return /^https?:\/\//i.test(value) || value.startsWith("/") || value.startsWith("data:image") || value.startsWith("blob:");
};

const parsePeriod = (period: string) => {
  const [rawStart, rawEnd] = period.split(" - ").map((part) => part.trim());
  const start = new Date(rawStart);
  const end = new Date(rawEnd);
  return {
    start,
    end,
    valid: !Number.isNaN(start.getTime()) && !Number.isNaN(end.getTime()),
  };
};

const formatDate = (value: Date) =>
  value.toLocaleDateString("sr-RS", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  });

const formatFloweringPeriod = (period: string) => {
  const { start, end, valid } = parsePeriod(period);
  if (!valid) return period;
  return `${formatDate(start)} - ${formatDate(end)}`;
};

const getCurrentCrop = (field: NearbyField) => {
  if (field.crops.length === 0) return null;
  const now = new Date();
  const active = field.crops.find((crop) => {
    const { start, end, valid } = parsePeriod(crop.floweringPeriod);
    if (!valid) return false;
    return now >= start && now <= end;
  });
  return active ?? field.crops[0];
};

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
      (() => {
        const currentCrop = getCurrentCrop(selectedField);
        const currentCropHasImage = isLikelyImageUrl(currentCrop?.icon);

        return (
      <div className="field-info-panel">
        <div className="field-info-title-wrap">
          <h4 className="field-info-title">{selectedField.fieldName}</h4>
          <div className="field-info-coords">
            {Number(selectedField.latitude).toFixed(5)}, {Number(selectedField.longitude).toFixed(5)}
          </div>
        </div>

        <div className="field-info-section">
          <div className="field-info-row">
            <span>Owner</span>
            <strong>{selectedField.owner.firstName} {selectedField.owner.lastName}</strong>
          </div>
          <div className="field-info-row">
            <span>Email</span>
            <strong>{selectedField.owner.email}</strong>
          </div>
          <div className="field-info-row">
            <span>Phone</span>
            <strong>{selectedField.owner.phone}</strong>
          </div>
        </div>

        <div className="field-info-section">
          <div className="field-info-section-title">Current crop</div>
          {!currentCrop ? (
            <div className="field-info-empty-crops">No crops</div>
          ) : (
            <div className="field-info-current-crop">
              {currentCropHasImage ? (
                <img
                  src={currentCrop.icon}
                  alt={currentCrop.name}
                  className="field-info-current-crop-image"
                  onError={(event) => {
                    event.currentTarget.style.display = "none";
                  }}
                />
              ) : (
                <div className="field-info-current-crop-fallback">{currentCrop.name}</div>
              )}
              <div className="field-info-current-crop-meta">
                <span className="field-info-current-crop-name">{currentCrop.name}</span>
                <span className="field-info-current-crop-period">{formatFloweringPeriod(currentCrop.floweringPeriod)}</span>
              </div>
            </div>
          )}
        </div>

        {(selectedField.thumbnailUrl || selectedField.imageUrl) && (
          <img
            className="field-info-image"
            src={selectedField.thumbnailUrl || selectedField.imageUrl}
            alt={selectedField.fieldName}
          />
        )}
      </div>
        );
      })()
    ) : null}
  </aside>
);

export default ApiaryInfoPanel;
