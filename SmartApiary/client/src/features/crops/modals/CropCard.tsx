import React from "react";
import "../layout/CropCard.css";
import type { Crop } from "../types/Crop";

interface CropCardProps {
  crop: Crop;
  cropNames: Record<number, string>;
  onHarvest: (id: string) => void; 
  onOpenCropDetails: (crop: Crop) => void;
}

const safeFormatDate = (dateStr?: string) => {
  if (!dateStr) return "N/A";
  const normalized = dateStr.replace(" ", "T");
  const d = new Date(normalized);
  return isNaN(d.getTime()) ? "N/A" : d.toLocaleDateString();
};



const CropCard: React.FC<CropCardProps> = ({ crop, cropNames, onHarvest, onOpenCropDetails }) => {
  const statusLabel = crop.isDeleted ? "Harvested" : "Planted";

  const handleCardClick = () => {
    onOpenCropDetails(crop);
  };

  return (
    <div className="crop-card" onClick={handleCardClick}>
      <div className="crop-card-header">
        <h4 className="crop-card-title">{cropNames[crop.cropName] || "Unknown Crop"}</h4>
      </div>
      <div className="crop-card-body">
        <p className="crop-card-desc">{crop.description || "No description"}</p>
        <div className="crop-card-period">
          Flowering: {safeFormatDate(crop.floweringPeriodStart)} – {safeFormatDate(crop.floweringPeriodEnd)}
        </div>
        <div className="crop-card-status">
          Status: <strong>{statusLabel}</strong>
        </div>
      </div>

      <div className="crop-card-footer">
        <button
          className="harvest-btn"
          onClick={() => onHarvest(crop.id)}
          disabled={crop.isDeleted}
        >
          Harvest
        </button>
      </div>
    </div>
  );
};

export default CropCard;
