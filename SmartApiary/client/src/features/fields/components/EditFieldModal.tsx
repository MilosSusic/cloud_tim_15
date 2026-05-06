import React, { useState } from "react";
import "../layout/Field.css";
import type { Field } from "../types/Field";

interface EditFieldFormData {
  id: string;
  fieldName: string;
  description: string;
  latitude: number;
  longitude: number;
}

const EditFieldModal: React.FC<{
  field: Field;
  onClose: () => void;
  onSave: (data: EditFieldFormData) => void;
}> = ({ field, onClose, onSave }) => {
  const [formData, setFormData] = useState<EditFieldFormData>({
    id: field.id,
    fieldName: field.fieldName,
    description: field.description ?? "",
    latitude: field.latitude,
    longitude: field.longitude,
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === "latitude" || name === "longitude" ? parseFloat(value) : value,
    }));
  };

  const handleSubmit = () => {
    onSave(formData);
    onClose();
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <h2>Edit Field</h2>
        <label>
          Field Name:
          <input type="text" name="fieldName" value={formData.fieldName} onChange={handleChange} />
        </label>
        <label>
          Description:
          <textarea name="description" value={formData.description} onChange={handleChange} />
        </label>
        <label>
          Latitude:
          <input type="number" name="latitude" value={formData.latitude} onChange={handleChange} />
        </label>
        <label>
          Longitude:
          <input type="number" name="longitude" value={formData.longitude} onChange={handleChange} />
        </label>

        <div className="modal-actions">
          <button onClick={handleSubmit}>Save</button>
          <button className="action-btn secondary" onClick={onClose}>Cancel</button>
        </div>
      </div>
    </div>
  );
};

export default EditFieldModal;