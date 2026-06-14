import React, { useState } from "react";
import "../layout/Crop.css"
import { CropType } from "../types/CropType";

export interface CropFormData {
  fieldId: string;
  cropName: CropType;
  description: string;
  FloweringPeriodStart: string;
  FloweringPeriodEnd: string; 
}

const formatDate = (date: Date): string => date.toISOString().split("T")[0];

const getTomorrow = (): string => {
  const tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);
  return formatDate(tomorrow);
};

const AddCropModal: React.FC<{ fieldId: string; onClose: () => void; onSave: (data: CropFormData,file: File | null) => Promise<void> }> = ({ fieldId, onClose, onSave }) => {
  const [formData, setFormData] = useState<CropFormData>({
    fieldId,
    cropName: CropType.None,
    description: "",
    FloweringPeriodStart: formatDate(new Date()),
    FloweringPeriodEnd: getTomorrow(),
  });

  const [file, setFile] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [error, setError] = useState<string>("");

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      const image = e.target.files[0];
      setFile(image);
      setPreviewUrl(URL.createObjectURL(image));
    }
  };


  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;

    if (name === "cropName") {
      setFormData((prev) => ({ ...prev, cropName: Number(value) as CropType }));
    } else {
      setFormData((prev) => ({ ...prev, [name]: value }));
    }
  };

  const handleSubmit = async () => {
    setError("");

    const start = new Date(formData.FloweringPeriodStart);
    const end = new Date(formData.FloweringPeriodEnd);

    if (Number.isNaN(start.getTime()) || Number.isNaN(end.getTime())) {
      setError("Please enter valid flowering dates.");
      return;
    }

    if (end <= start) {
      setError("Flowering period end must be after start date.");
      return;
    }

    try {
      await onSave(formData, file);
      onClose();
    } catch {
      // Parent already sets a toast message; keep modal open so user can correct input.
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <h2>Add New Crop</h2>
        <label>
          Crop Name:
          <select name="cropName" value={formData.cropName} onChange={handleChange}>
            {Object.entries(CropType)
              .filter(([, val]) => typeof val === "number") 
              .map(([key, val]) => (
                <option key={val} value={val}>
                  {key}
                </option>
              ))}
          </select>
        </label>
        <label>
          Description:
          <textarea name="description" value={formData.description} onChange={handleChange} />
        </label>
        <label>
          Flowering Period Start:
          <input
            type="date"
            name="FloweringPeriodStart"
            value={formData.FloweringPeriodStart}
            onChange={handleChange}
          />
        </label>
        <label>
          Flowering Period End:
          <input
            type="date"
            name="FloweringPeriodEnd"
            value={formData.FloweringPeriodEnd}
            min={formData.FloweringPeriodStart}
            onChange={handleChange}
          />
        </label>
        <label>
          Image:
          <input type="file" accept="image/*" onChange={handleFileChange} />
        </label>
        {previewUrl && <img src={previewUrl} alt="Preview" style={{ maxWidth: "200px" }} />}
        {error && <p style={{ color: "#f87171", margin: "8px 0 0" }}>{error}</p>}

        <div className="modal-actions">
          <button onClick={handleSubmit}>Add</button>
          <button onClick={onClose}>Cancel</button>
        </div>
      </div>
    </div>
  );
};

export default AddCropModal;
