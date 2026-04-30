import React, { useState, useEffect } from "react";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import { getFields, createField } from "../api/fieldApi";
import { createCrop, getCropsByField } from "../../crops/api/cropApi"; 
import { changeSprayStatus, createSpray, getSprayByField, rescheduleSpray, updateSpray } from "../../spraySchedule/api/sprayScheduleApi";
import AddFieldModal from "../modals/AddFieldModal";
import AddCropModal from "../../crops/modals/AddCropModal";
import SprayScheduleModal from "../../spraySchedule/modals/SprayScheduleModal";
import UpdateSprayScheduleModal from "../../spraySchedule/modals/UpdateSprayScheduleModal";
import type { Crop } from "../../crops/types/Crop";
import type { SpraySchedule } from "../../spraySchedule/types/SpraySchedule";
import "leaflet/dist/leaflet.css";

import type { Field } from "../types/Field";
import type { FieldFormData } from "../modals/AddFieldModal";
import type { CropFormData } from "../../crops/modals/AddCropModal";
import type { SprayScheduleFormData } from "../../spraySchedule/modals/SprayScheduleModal";
import type { UpdateSprayScheduleFormData } from "../../spraySchedule/modals/UpdateSprayScheduleModal";
import SprayCard from "../../spraySchedule/modals/SprayCard";
import { SprayStatus } from "../../spraySchedule/types/SprayStatus";



const FieldPage: React.FC = () => {
  const [fields, setFields] = useState<Field[]>([]);
  const [selectedField, setSelectedField] = useState<Field | null>(null);
  const [crops, setCrops] = useState<Crop[]>([]);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [showFieldModal, setShowFieldModal] = useState(false);
  const [showCropModal, setShowCropModal] = useState(false);
  const [showSprayModal, setShowSprayModal] = useState(false);
  const [sprays, setSprays] = useState<SpraySchedule[]>([]);
  const [sprayToUpdate, setSprayToUpdate] = useState<UpdateSprayScheduleFormData | null>(null);
  const [activeTab, setActiveTab] = useState<"crops" | "sprays">("crops");

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
      console.log("Fetched crops:", data);
      setCrops(data);
      setMessage("Successfully fetched crops.");
    } catch (err) {
      console.error("Error fetching crops:", err);
      setMessage("Failed to fetch crops.");
    }
    setLoading(false);
  };

  const handleRescheduleSpray = async (id: string, newDate: string) => {
    try {
      await rescheduleSpray(id.toString(), newDate);
      setMessage("Spray schedule rescheduled successfully.");
      
      if (selectedField) {
        fetchSprays(selectedField.id);
      } 
    } catch (error) {
      console.error("Error rescheduling spray:", error);
    }
  };

  const handleCompleteSpray = async (id: string) => {
  try {
    console.log(`Completing spray with ID: ${id}`);
    await changeSprayStatus(id, SprayStatus.Completed);
      if (selectedField) {
        fetchSprays(selectedField.id);
      } 
    } catch (error) {
      console.error("Error completing spray:", error);
    }
  };

  const handleCancelSpray = async (id: string) => {
  try {
    console.log(`Cancelling spray with ID: ${id}`);
    await changeSprayStatus(id, SprayStatus.Cancelled);
      if (selectedField) {
        fetchSprays(selectedField.id);
      } 
    } catch (error) {
      console.error("Error cancelling spray:", error);
    }
  };
    

  const fetchSprays = async (fieldId: string | number) => {
    setLoading(true);
    try {
        const data = await getSprayByField(fieldId.toString());
        console.log("Fetched sprays:", data);
        setSprays(data);
        setMessage("Successfully fetched sprays.");
    } catch (err) {
        console.error("Error fetching sprays:", err);
        setMessage("Failed to fetch sprays.");
    }
    setLoading(false);
    };

  const handleAddField = () => {
    setShowFieldModal(true);
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
        await updateSpray(data.id,data); 
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

  return (
  <div className="field-page" style={{ display: "flex", height: "100vh" }}>
    {/* Leva strana: lista polja */}
    <aside
      className="field-list"
      style={{
        width: "300px",
        borderRight: "1px solid #ccc",
        padding: "10px",
        overflowY: "auto",
      }}
    >
      <header
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
        }}
      >
        <h3>Fields</h3>
        <button className="action-btn" onClick={handleAddField}>+ Add Field</button>
        {showFieldModal && (
          <AddFieldModal
            onClose={() => setShowFieldModal(false)}
            onSave={handleSaveField}
          />
        )}
      </header>
      {loading && <p>Loading fields...</p>}
      <p>{message}</p>
      <ul style={{ listStyle: "none", padding: 0 }}>
        {fields.map((field) => (
          <li
            key={field.id}
            style={{
              marginBottom: "10px",
              cursor: "pointer",
              background:
                selectedField?.id === field.id ? "#eef" : "transparent",
              padding: "5px",
              borderRadius: "4px",
            }}
            onClick={() => setSelectedField(field)}
          >
            <strong>{field.fieldName}</strong>
            <div style={{ fontSize: "0.9em", color: "#555" }}>
              {field.description}
            </div>
            <div style={{ fontSize: "0.8em", color: "#777" }}>
              Lat: {field.latitude}, Lng: {field.longitude}
            </div>
          </li>
        ))}
      </ul>
    </aside>

    {/* Centar: mapa + desni sidebar */}
    <section
      className="field-map"
      style={{ flex: 1, display: "flex", position: "relative" }}
    >
      <MapContainer
        center={[45.25, 19.85]}
        zoom={6}
        style={{ height: "100%", width: "100%" }}
      >
        <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />

        {fields.map((field) => (
          <Marker
            key={field.id}
            position={[field.latitude, field.longitude]}
            eventHandlers={{ click: () => setSelectedField(field) }}
          >
            <Popup>
              <div className="field-popup">
                <img
                  src={field.thumbnailUrl || field.imageUrl}
                  alt={field.fieldName}
                  style={{ width: "100px", height: "auto" }}
                />
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

      {/* Desna strana: sidebar sa tabovima */}
      {selectedField && (
        <aside className="crop-list">
          <header className="sidebar-header">
            <h4>
              {activeTab === "crops" ? "Crops" : "Spray Schedules"} for{" "}
              {selectedField.fieldName}
            </h4>
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
          </header>

          {/* CROPS TAB */}
          {activeTab === "crops" && (
            <>
              <button className="action-btn" onClick={() => setShowCropModal(true)}>+ Add Crop</button>
              {showCropModal && (
                <AddCropModal
                  fieldId={selectedField.id}
                  onClose={() => setShowCropModal(false)}
                  onSave={handleSaveCrop}
                />
              )}
              {crops.length > 0 ? (
                <ul className="sidebar-list">
                  {crops.map((crop) => (
                    <li key={crop.id}>
                      <strong>{cropNames[crop.cropName]}</strong>
                      <div>{crop.description}</div>
                      <div style={{ color: crop.color }}>
                        Color: {crop.color}
                      </div>
                      {crop.note && <div>Note: {crop.note}</div>}
                    </li>
                  ))}
                </ul>
              ) : (
                <p>No crops for this field.</p>
              )}
            </>
          )}

          {/* SPRAYS TAB */}
          {activeTab === "sprays" && (
            <>
              <button className="action-btn" onClick={() => setShowSprayModal(true)}>+ Schedule Spray</button>
              {showSprayModal && (
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
              {sprays.length > 0 ? (
                <div className="sidebar-list">
                  {sprays.map((spray) => (
                    <SprayCard
                      key={spray.id}
                      spray={spray}
                      onCancel={(id) => handleCancelSpray(id)}
                      onComplete={(id) => handleCompleteSpray(id)}
                      onReschedule={(id, newDate) => handleRescheduleSpray(id, newDate)}
                    />
                  ))}
                </div>
              ) : (
                <p>No spray schedules for this field.</p>
              )}
            </>
          )}  
        </aside>
      )}
    </section>
  </div>
);

};

export default FieldPage;
