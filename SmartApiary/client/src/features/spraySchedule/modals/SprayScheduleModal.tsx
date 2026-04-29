import React, { useState } from "react";
import "../layout/Spray.css"

interface SprayScheduleFormData {
  fieldId: string;
  startDateTime: string;
  durationHours: number;
  pesticideType: string;
}

const SprayScheduleModal: React.FC<{
  fieldId: string;
  onClose: () => void;
  onSave: (data: SprayScheduleFormData) => void;
}> = ({ fieldId, onClose, onSave }) => {
  const [formData, setFormData] = useState<SprayScheduleFormData>({
    fieldId,
    startDateTime: "",
    durationHours: 1,
    pesticideType: "",
  });

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = () => {
    onSave(formData);
    onClose();
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <h2>Schedule Spray</h2>

        <label>
          Start Date & Time:
          <input
            type="datetime-local"
            name="startDateTime"
            value={formData.startDateTime}
            onChange={handleChange}
          />
        </label>

        <label>
          Duration (hours):
          <input
            type="number"
            name="durationHours"
            value={formData.durationHours}
            onChange={handleChange}
          />
        </label>

        <label>
          Pesticide Type:
          <input
            type="text"
            name="pesticideType"
            value={formData.pesticideType}
            onChange={handleChange}
          />
        </label>

        <div className="modal-actions">
          <button onClick={handleSubmit}>Save</button>
          <button onClick={onClose}>Cancel</button>
        </div>
      </div>
    </div>
  );
};

export default SprayScheduleModal;
