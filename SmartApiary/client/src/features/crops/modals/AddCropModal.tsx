import React, { useState } from "react";
import "../layout/Crop.css"
import { CropType } from "../types/CropType";

interface CropFormData {
  fieldId: number;
  cropName: CropType;
  description: string;
  note?: string;
  color?: string;
}

const AddCropModal: React.FC<{ fieldId: number; onClose: () => void; onSave: (data: CropFormData) => void }> = ({ fieldId, onClose, onSave }) => {
  const [formData, setFormData] = useState<CropFormData>({
    fieldId,
    cropName: CropType.None,
    description: "",
    note: "",
    color: "",
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;

    if (name === "cropName") {
      setFormData((prev) => ({ ...prev, cropName: Number(value) as CropType }));
    } else {
      setFormData((prev) => ({ ...prev, [name]: value }));
    }
  };

  const handleSubmit = () => {
    onSave(formData);
    onClose();
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <h2>Add New Crop</h2>
        <label>
          Crop Name:
          <select name="cropName" value={formData.cropName} onChange={handleChange}>
            {Object.entries(CropType)
              .filter(([key, val]) => typeof val === "number") // uzmi samo numeric vrednosti
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
          Note:
          <input type="text" name="note" value={formData.note} onChange={handleChange} />
        </label>
        <label>
          Color:
          <input type="text" name="color" value={formData.color} onChange={handleChange} />
        </label>

        <div className="modal-actions">
          <button onClick={handleSubmit}>Add</button>
          <button onClick={onClose}>Cancel</button>
        </div>
      </div>
    </div>
  );
};

export default AddCropModal;
