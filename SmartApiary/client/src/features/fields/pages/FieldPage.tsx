import React, { useEffect, useState } from "react";
import { MapContainer, Marker, Popup, TileLayer } from "react-leaflet";
import { createCrop, getCropsByField } from "../../crops/api/cropApi";
import AddCropModal from "../../crops/modals/AddCropModal";
import type { Crop } from "../../crops/types/Crop";
import {
  changeSprayStatus,
  createSpray,
  getSprayByField,
  rescheduleSpray,
  updateSpray,
} from "../../spraySchedule/api/sprayScheduleApi";
import SprayCard from "../../spraySchedule/modals/SprayCard";
import SprayScheduleModal from "../../spraySchedule/modals/SprayScheduleModal";
import UpdateSprayScheduleModal from "../../spraySchedule/modals/UpdateSprayScheduleModal";
import { SprayStatus } from "../../spraySchedule/types/SprayStatus";
import type { SpraySchedule } from "../../spraySchedule/types/SpraySchedule";
import { createField, getFields } from "../api/fieldApi";
import "../layout/Field.css";
import AddFieldModal from "../modals/AddFieldModal";
import type { FieldFormData } from "../modals/AddFieldModal";
import type { Field } from "../types/Field";
import type { CropFormData } from "../../crops/modals/AddCropModal";
import type { SprayScheduleFormData } from "../../spraySchedule/modals/SprayScheduleModal";
import type { UpdateSprayScheduleFormData } from "../../spraySchedule/modals/UpdateSprayScheduleModal";
import "leaflet/dist/leaflet.css";

const cropNames: Record<number, string> = {
  1: "Wheat",
  2: "Rice",
  3: "Barley",
  4: "Oats",
  5: "Corn",
  6: "Sorghum",
  7: "Buckwheat",
  8: "Chia",
};

const FieldPage: React.FC = () => {
  const [fields, setFields] = useState<Field[]>([]);
  const [selectedField, setSelectedField] = useState<Field | null>(null);
  const [crops, setCrops] = useState<Crop[]>([]);
  const [sprays, setSprays] = useState<SpraySchedule[]>([]);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [showFieldModal, setShowFieldModal] = useState(false);
  const [showCropModal, setShowCropModal] = useState(false);
  const [showSprayModal, setShowSprayModal] = useState(false);
  const [sprayToUpdate, setSprayToUpdate] =
    useState<UpdateSprayScheduleFormData | null>(null);
  const [activeTab, setActiveTab] = useState<"crops" | "sprays">("crops");

  const isErrorMessage = (text: string) => {
    const normalized = text.toLowerCase();
    return (
      normalized.includes("fail") ||
      normalized.includes("error") ||
      normalized.includes("gre")
    );
  };

  const fetchFields = async () => {
    setLoading(true);
    try {
      const data = await getFields();
      setFields(data);
      setMessage("Successfully fetched fields.");
    } catch (err) {
      console.error("Error fetching fields:", err);
      setMessage("Failed to fetch fields.");
    }
    setLoading(false);
  };

  const fetchCrops = async (fieldId: string | number) => {
    setLoading(true);
    try {
      const data = await getCropsByField(fieldId.toString());
      setCrops(data);
      setMessage("Successfully fetched crops.");
    } catch (err) {
      console.error("Error fetching crops:", err);
      setMessage("Failed to fetch crops.");
    }
    setLoading(false);
  };

  const fetchSprays = async (fieldId: string | number) => {
    setLoading(true);
    try {
      const data = await getSprayByField(fieldId.toString());
      setSprays(data);
      setMessage("Successfully fetched sprays.");
    } catch (err) {
      console.error("Error fetching sprays:", err);
      setMessage("Failed to fetch sprays.");
    }
    setLoading(false);
  };

  const handleRescheduleSpray = async (id: string, newDate: string) => {
    try {
      await rescheduleSpray(id, newDate);
      setMessage("Spray schedule rescheduled successfully.");
      if (selectedField) {
        await fetchSprays(selectedField.id);
      }
    } catch (error) {
      console.error("Error rescheduling spray:", error);
      setMessage("Failed to reschedule spray schedule.");
    }
  };

  const handleCompleteSpray = async (id: string) => {
    try {
      await changeSprayStatus(id, SprayStatus.Completed);
      setMessage("Spray marked as completed.");
      if (selectedField) {
        await fetchSprays(selectedField.id);
      }
    } catch (error) {
      console.error("Error completing spray:", error);
      setMessage("Failed to complete spray.");
    }
  };

  const handleCancelSpray = async (id: string) => {
    try {
      await changeSprayStatus(id, SprayStatus.Cancelled);
      setMessage("Spray cancelled successfully.");
      if (selectedField) {
        await fetchSprays(selectedField.id);
      }
    } catch (error) {
      console.error("Error cancelling spray:", error);
      setMessage("Failed to cancel spray.");
    }
  };

  const handleSaveField = async (data: FieldFormData) => {
    setLoading(true);
    try {
      await createField(data);
      setShowFieldModal(false);
      setMessage("Field created successfully.");
      await fetchFields();
    } catch (err) {
      console.error("Error creating field:", err);
      setMessage("Failed to create field.");
    }
    setLoading(false);
  };

  const handleSaveCrop = async (data: CropFormData) => {
    setLoading(true);
    try {
      await createCrop(data);
      setShowCropModal(false);
      setMessage("Crop created successfully.");
      if (selectedField) {
        await fetchCrops(selectedField.id);
      }
    } catch (err) {
      console.error("Error creating crop:", err);
      setMessage("Failed to create crop.");
    }
    setLoading(false);
  };

  const handleSaveSpray = async (data: SprayScheduleFormData) => {
    setLoading(true);
    try {
      const payload = {
        ...data,
        fieldId: selectedField?.id,
        createdAt: new Date().toISOString(),
      };
      await createSpray(payload);
      setShowSprayModal(false);
      setMessage("Spray schedule created successfully.");
      if (selectedField) {
        await fetchSprays(selectedField.id);
      }
    } catch (err) {
      console.error("Error creating spray schedule:", err);
      setMessage("Failed to create spray schedule.");
    }
    setLoading(false);
  };

  const handleUpdateSpray = async (data: UpdateSprayScheduleFormData) => {
    try {
      await updateSpray(data.id, data);
      setMessage("Spray schedule updated successfully.");
      setSprayToUpdate(null);
      if (selectedField) {
        await fetchSprays(selectedField.id);
      }
    } catch (err) {
      console.error("Error updating spray schedule:", err);
      setMessage("Failed to update spray schedule.");
    }
  };

  useEffect(() => {
    fetchFields();
  }, []);

  useEffect(() => {
    if (selectedField) {
      fetchCrops(selectedField.id);
      fetchSprays(selectedField.id);
    } else {
      setCrops([]);
      setSprays([]);
    }
  }, [selectedField]);

  const sprayPriority = (status: number) => {
    switch (status) {
      case SprayStatus.Scheduled:
        return 0;
      case SprayStatus.Rescheduled:
        return 1;
      case SprayStatus.Cancelled:
        return 2;
      case SprayStatus.Completed:
        return 3;
      default:
        return 4;
    }
  };

  const sortedSprays = [...sprays].sort((left, right) => {
    const priorityDiff = sprayPriority(left.status) - sprayPriority(right.status);
    if (priorityDiff !== 0) {
      return priorityDiff;
    }

    return (
      new Date(left.startDateTime).getTime() - new Date(right.startDateTime).getTime()
    );
  });

  const visibleSprays = sortedSprays.filter(
    (spray) => spray.status !== SprayStatus.Completed
  );
  const completedSprays = sortedSprays
    .filter((spray) => spray.status === SprayStatus.Completed)
    .sort(
      (left, right) =>
        new Date(right.startDateTime).getTime() - new Date(left.startDateTime).getTime()
    );

  return (
    <div className="field-page">
      <div className="field-header">
        <div>
          <div className="field-header-eyebrow">Smart Agriculture</div>
          <h2>My Fields</h2>
        </div>
        <button className="action-btn" onClick={() => setShowFieldModal(true)}>
          + Add Field
        </button>
      </div>

      {message && (
        <div className={`field-toast ${isErrorMessage(message) ? "error" : "success"}`}>
          <span className="field-toast-dot" />
          {message}
        </div>
      )}

      <div className="field-grid">
        <aside className="field-list-card">
          <div className="field-list-card-header">
            <h3>Parcels</h3>
            <span className="field-list-count-badge">{fields.length}</span>
          </div>

          {loading && fields.length === 0 && <div className="field-list-empty">Loading fields...</div>}
          {!loading && fields.length === 0 && <div className="field-list-empty">No fields yet.</div>}

          <ul className="field-list-items">
            {fields.map((field) => (
              <li
                key={field.id}
                className={`field-list-item ${selectedField?.id === field.id ? "active" : ""}`}
                onClick={() => setSelectedField(field)}
              >
                <div className="field-list-title">{field.fieldName}</div>
                <div className="field-list-desc">{field.description || "No description"}</div>
                <div className="field-list-meta">
                  {Number(field.latitude).toFixed(4)}, {Number(field.longitude).toFixed(4)}
                </div>
              </li>
            ))}
          </ul>
        </aside>

        <section className="field-map-card">
          <MapContainer center={[45.25, 19.85]} zoom={6} style={{ height: "100%", width: "100%" }}>
            <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
            {fields.map((field) => (
              <Marker
                key={field.id}
                position={[field.latitude, field.longitude]}
                eventHandlers={{ click: () => setSelectedField(field) }}
              >
                <Popup>
                  <div className="field-popup">
                    {(field.thumbnailUrl || field.imageUrl) && (
                      <img src={field.thumbnailUrl || field.imageUrl} alt={field.fieldName} />
                    )}
                    <div className="field-popup-name">{field.fieldName}</div>
                    <p className="field-popup-desc">{field.description}</p>
                    <div className="field-popup-coords">
                      Lat: {field.latitude}, Lng: {field.longitude}
                    </div>
                  </div>
                </Popup>
              </Marker>
            ))}
          </MapContainer>
        </section>

        <aside className="field-side-panel">
          {!selectedField ? (
            <div className="field-side-empty">
              Select a parcel from the list or map to manage crops and spray plans.
            </div>
          ) : (
            <>
              {(selectedField.thumbnailUrl || selectedField.imageUrl) && (
                <img
                  className="field-side-image"
                  src={selectedField.thumbnailUrl || selectedField.imageUrl}
                  alt={selectedField.fieldName}
                />
              )}

              <div className="field-side-details">
                <div className="field-side-title">{selectedField.fieldName}</div>
                <div className="field-side-coords">
                  {Number(selectedField.latitude).toFixed(5)}, {Number(selectedField.longitude).toFixed(5)}
                </div>
                {selectedField.description && (
                  <p className="field-side-description">{selectedField.description}</p>
                )}
              </div>

              <div className="sidebar-tabs">
                <button
                  className={activeTab === "crops" ? "active" : ""}
                  onClick={() => setActiveTab("crops")}
                >
                  Crops
                </button>
                <button
                  className={activeTab === "sprays" ? "active" : ""}
                  onClick={() => setActiveTab("sprays")}
                >
                  Sprays
                </button>
              </div>

              <div className="field-side-scroll">
                {activeTab === "crops" && (
                  <>
                    <button className="action-btn" onClick={() => setShowCropModal(true)}>
                      + Add Crop
                    </button>
                    <div className="field-side-list-scroll">
                      {crops.length > 0 ? (
                        <ul className="sidebar-list">
                          {crops.map((crop) => (
                            <li key={crop.id} className="field-crop-item">
                              <div className="field-crop-name">{cropNames[crop.cropName] || "Unknown Crop"}</div>
                              <div className="field-crop-desc">{crop.description || "No description"}</div>
                              <div className="field-crop-color" style={{ color: crop.color }}>
                                Color: {crop.color}
                              </div>
                              {crop.note && <div className="field-crop-note">Note: {crop.note}</div>}
                            </li>
                          ))}
                        </ul>
                      ) : (
                        <p className="field-panel-empty">No crops for this field.</p>
                      )}
                    </div>
                  </>
                )}

                {activeTab === "sprays" && (
                  <>
                    <button className="action-btn" onClick={() => setShowSprayModal(true)}>
                      + Schedule Spray
                    </button>
                    <div className="field-side-list-scroll">
                      {sprays.length > 0 ? (
                        <div className="sidebar-list">
                          {visibleSprays.length > 0 && (
                            <div className="field-spray-group">
                              <div className="field-spray-group-title">
                                Active and scheduled
                                <span>{visibleSprays.length}</span>
                              </div>
                              {visibleSprays.map((spray) => (
                                <SprayCard
                                  key={spray.id}
                                  spray={spray}
                                  onCancel={(id) => handleCancelSpray(id)}
                                  onComplete={(id) => handleCompleteSpray(id)}
                                  onReschedule={(id, newDate) => handleRescheduleSpray(id, newDate)}
                                />
                              ))}
                            </div>
                          )}

                          {completedSprays.length > 0 && (
                            <div className="field-spray-group field-spray-group-completed">
                              <div className="field-spray-group-title">
                                Completed
                                <span>{completedSprays.length}</span>
                              </div>
                              {completedSprays.map((spray) => (
                                <SprayCard
                                  key={spray.id}
                                  spray={spray}
                                  onCancel={(id) => handleCancelSpray(id)}
                                  onComplete={(id) => handleCompleteSpray(id)}
                                  onReschedule={(id, newDate) => handleRescheduleSpray(id, newDate)}
                                />
                              ))}
                            </div>
                          )}
                        </div>
                      ) : (
                        <p className="field-panel-empty">No spray schedules for this field.</p>
                      )}
                    </div>
                  </>
                )}
              </div>
            </>
          )}
        </aside>
      </div>

      {showFieldModal && (
        <AddFieldModal onClose={() => setShowFieldModal(false)} onSave={handleSaveField} />
      )}

      {showCropModal && selectedField && (
        <AddCropModal
          fieldId={selectedField.id}
          onClose={() => setShowCropModal(false)}
          onSave={handleSaveCrop}
        />
      )}

      {showSprayModal && selectedField && (
        <SprayScheduleModal
          fieldId={selectedField.id}
          onClose={() => setShowSprayModal(false)}
          onSave={handleSaveSpray}
        />
      )}

      {sprayToUpdate && (
        <UpdateSprayScheduleModal
          spray={sprayToUpdate}
          onClose={() => setSprayToUpdate(null)}
          onSave={handleUpdateSpray}
        />
      )}
    </div>
  );
};

export default FieldPage;
