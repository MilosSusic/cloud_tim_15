import React from "react";
import "../layout/CropDetails.css";
import type { Crop } from "../types/Crop";

interface CropDetailsProps {
  crop: Crop;
  cropNames: Record<number, string>;
  onClose: () => void;
  onHarvest: (id: string) => void;
}

const safeFormatDate = (dateStr?: string) => {
  if (!dateStr) return "N/A";
  const normalized = dateStr.replace(" ", "T");
  const d = new Date(normalized);
  return isNaN(d.getTime()) ? "N/A" : d.toLocaleDateString();
};

const CropDetails: React.FC<CropDetailsProps> = ({ crop, cropNames, onClose, onHarvest }) => {
  const statusLabel = crop.isDeleted ? "Harvested" : "Planted";

  return (
    <div className="crop-details-overlay">
      <div className="crop-details-card">
        <button className="close-btn" onClick={onClose}>×</button>
        <h2 className="crop-details-title">{cropNames[crop.cropName] || "Unknown Crop"}</h2>
        <div className="crop-details-body">
          <p><strong>Description:</strong> {crop.description || "No description"}</p>
          <div className="crop-card-period">
            Flowering: {safeFormatDate(crop.floweringPeriodStart)} – {safeFormatDate(crop.floweringPeriodEnd)}
          </div>
          <div className="crop-card-status">
            Status: <strong>{statusLabel}</strong>
          </div>
        </div>

        <div className="crop-details-footer">
          <button
            className="harvest-btn"
            onClick={() => onHarvest(crop.id)}
            disabled={crop.isDeleted}
          >
            Harvest
          </button>
        </div>
      </div>
    </div>
  );
};

export default CropDetails;
