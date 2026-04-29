import React, { useState } from "react";
import "../layout/Field.css"

interface FieldFormData {
  fieldName: string;
  description: string;
  latitude: number;
  longitude: number;
  file?: File;
}

const AddFieldModal: React.FC<{ onClose: () => void; onSave: (data: FieldFormData) => void }> = ({ onClose, onSave }) => {
  const [formData, setFormData] = useState<FieldFormData>({
    fieldName: "",
    description: "",
    latitude: 0,
    longitude: 0,
    file: undefined,
  });
  const [imagePreview, setImagePreview] = useState<string | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === "latitude" || name === "longitude" ? parseFloat(value) : value,
    }));
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      const file = e.target.files[0];
      setFormData((prev) => ({ ...prev, file }));
      setImagePreview(URL.createObjectURL(file));
    }
  };

  const handleSubmit = () => {
    onSave(formData);
    onClose();
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <h2>Add New Field</h2>
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
        <label>
          Image:
          <input type="file" accept="image/*" onChange={handleFileChange} />
        </label>

        {imagePreview && (
          <div className="image-preview">
            <p>Image preview:</p>
            <img src={imagePreview} alt="Preview" />
          </div>
        )}

        <div className="modal-actions">
          <button onClick={handleSubmit}>Add</button>
          <button className="action-btn secondary" onClick={onClose}>Cancel</button>
        </div>
      </div>
    </div>
  );
};

export default AddFieldModal;
