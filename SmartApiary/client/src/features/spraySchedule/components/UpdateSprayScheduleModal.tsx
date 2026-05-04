import React, { useState } from "react";
import "../layout/Spray.css"

export interface UpdateSprayScheduleFormData {
  id: string;              // ID spray schedule-a koji se menja
  fieldId: string;         // ID polja
  startDateTime: string;
  durationHours: number;
  pesticideType: string;
  status: number;          // ENUM vrednost kao broj
}

const UpdateSprayScheduleModal: React.FC<{
  spray: UpdateSprayScheduleFormData;
  onClose: () => void;
  onSave: (data: UpdateSprayScheduleFormData) => void;
}> = ({ spray, onClose, onSave }) => {
  const [formData, setFormData] = useState<UpdateSprayScheduleFormData>(spray);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;

    // ako je status, konvertuj u number
    if (name === "status") {
      setFormData((prev) => ({ ...prev, status: Number(value) }));
    } else if (name === "durationHours") {
      setFormData((prev) => ({ ...prev, durationHours: Number(value) }));
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
        <h2>Update Spray Schedule</h2>

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

        <label>
          Status:
          <select
            name="status"
            value={formData.status}
            onChange={handleChange}
          >
            <option value={1}>1 - Scheduled</option>
            <option value={2}>2 - Cancelled</option>
            <option value={3}>3 - Completed</option>
            <option value={4}>4 - Rescheduled</option>
          </select>
        </label>

        <div className="modal-actions">
          <button onClick={handleSubmit}>Save</button>
          <button onClick={onClose}>Cancel</button>
        </div>
      </div>
    </div>
  );
};

export default UpdateSprayScheduleModal;
