// ErrorBoundary komponenta za hvatanje grešaka u MapContainer
class MapErrorBoundary extends React.Component<{children: React.ReactNode}, {hasError: boolean}> {
  constructor(props: any) {
    super(props);
    this.state = { hasError: false };
  }
  static getDerivedStateFromError() { return { hasError: true }; }
  componentDidCatch(error: any, info: any) { console.error('Map error:', error, info); }
  render() {
    if (this.state.hasError) return <div style={{color:'red'}}>Greška u prikazu mape.</div>;
    return this.props.children;
  }
}
import React, { useEffect, useState } from "react";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import { useApiaryValidation } from "../hooks/useApiaryValidation";
import L from "leaflet";
// Custom marker icons
const orangeIcon = new L.Icon({
  iconUrl: "https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-orange.png",
  shadowUrl: "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png",
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  shadowSize: [41, 41],
});
const greenIcon = new L.Icon({
  iconUrl: "https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-green.png",
  shadowUrl: "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png",
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  shadowSize: [41, 41],
});
import { getApiaries, createApiary, updateApiary } from "../api/apiaryApi";
import { getNearbyFieldsWithCrops } from "../api/nearbyFieldsApi";
//import type { NearbyField } from "../types/NearbyField";
import HivesSection from "../../hives/components/HivesSection";

import "leaflet/dist/leaflet.css";
import "../layout/ApiaryPage.css";

import type { Apiary } from "../types/Apiary";
import type { NearbyField } from "../types/NearbyField";

const ApiaryPage: React.FC = () => {
  const [apiaries, setApiaries] = useState<Apiary[]>([]);
  const [showForm, setShowForm] = useState(false);
  const [nearbyFields, setNearbyFields] = useState<NearbyField[]>([]);
  const [fieldsLoading, setFieldsLoading] = useState(false);
  const [selectedApiary, setSelectedApiary] = useState<Apiary | null>(null);
  const [editingApiary, setEditingApiary] = useState<Apiary | null>(null);
  const [selectedField, setSelectedField] = useState<NearbyField | null>(null);
  const [editForm, setEditForm] = useState({
    name: "",
    description: "",
    latitude: "",
    longitude: "",
    file: undefined as File | undefined,
  });
  const [editImagePreview, setEditImagePreview] = useState<string | null>(null);
  const [form, setForm] = useState({
    name: "",
    description: "",
    latitude: "",
    longitude: "",
    file: undefined as File | undefined,
  });
  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const createEmptyApiaryTouched = () => ({
    name: false,
    description: false,
    latitude: false,
    longitude: false,
  });
  const [createTouched, setCreateTouched] = useState(createEmptyApiaryTouched());
  const [editTouched, setEditTouched] = useState(createEmptyApiaryTouched());
  const [createSubmitted, setCreateSubmitted] = useState(false);
  const [editSubmitted, setEditSubmitted] = useState(false);

  const createValidation = useApiaryValidation(form);
  const editValidation = useApiaryValidation(editForm);

  const isErrorMessage = (text: string) => {
    const normalized = text.toLowerCase();
    return (
      normalized.includes("fail") ||
      normalized.includes("error") ||
      normalized.includes("gre")
    );
  };

  const shouldShowCreateError = (field: keyof typeof createTouched) =>
    createSubmitted || createTouched[field];
  const shouldShowEditError = (field: keyof typeof editTouched) =>
    editSubmitted || editTouched[field];

  useEffect(() => {
    fetchApiaries();
  }, []);

  useEffect(() => {
    if (apiaries.length > 0) {
      fetchNearbyFields();
    }
  }, [apiaries]);

  const fetchNearbyFields = async () => {
    setFieldsLoading(true);
    try {
      const data = await getNearbyFieldsWithCrops();
      setNearbyFields(data);
    } catch {
      setNearbyFields([]);
    }
    setFieldsLoading(false);
  };

  const fetchApiaries = async () => {
    setLoading(true);
    try {
      const data = await getApiaries();
      setApiaries(data);
      setMessage("Successfully fetched apiaries.");
    } catch {
      setMessage("Failed to fetch apiaries.");
    }
    setLoading(false);
  };

  const handleInputChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value, files } = e.target as any;
    if (name === "file") {
      const file = files[0];
      setForm({ ...form, file });
      if (file) {
        setImagePreview(URL.createObjectURL(file));
      } else {
        setImagePreview(null);
      }
    } else {
      setForm({ ...form, [name]: value });
      if (
        name === "name" ||
        name === "description" ||
        name === "latitude" ||
        name === "longitude"
      ) {
        setCreateTouched({ ...createTouched, [name]: true });
      }
    }
  };

  const openCreate = () => {
    setCreateTouched(createEmptyApiaryTouched());
    setCreateSubmitted(false);
    setShowForm(true);
  };

  const closeCreate = () => {
    setCreateTouched(createEmptyApiaryTouched());
    setCreateSubmitted(false);
    setShowForm(false);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setCreateSubmitted(true);
    if (!createValidation.isValid) return;
    setLoading(true);
    try {
      await createApiary(form);
      closeCreate();
      setForm({
        name: "",
        description: "",
        latitude: "",
        longitude: "",
        file: undefined,
      });
      setImagePreview(null);
      setMessage("Apiary created successfully.");
      await fetchApiaries();
      await fetchNearbyFields();
    } catch (err) {
      setMessage("Failed to create apiary.");
    }
    setLoading(false);
  };

  const openEdit = (apiary: Apiary) => {
    setEditingApiary(apiary);
    setEditTouched(createEmptyApiaryTouched());
    setEditSubmitted(false);
    setEditForm({
      name: apiary.name,
      description: apiary.description ?? "",
      latitude: String(apiary.latitude),
      longitude: String(apiary.longitude),
      file: undefined,
    });
    setEditImagePreview(null);
  };

  const closeEdit = () => {
    setEditingApiary(null);
    setEditImagePreview(null);
    setEditTouched(createEmptyApiaryTouched());
    setEditSubmitted(false);
  };

  const handleEditChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value, files } = e.target as any;
    if (name === "file") {
      const file = files[0];
      setEditForm({ ...editForm, file });
      setEditImagePreview(file ? URL.createObjectURL(file) : null);
    } else {
      setEditForm({ ...editForm, [name]: value });
      if (
        name === "name" ||
        name === "description" ||
        name === "latitude" ||
        name === "longitude"
      ) {
        setEditTouched({ ...editTouched, [name]: true });
      }
    }
  };

  const handleEditSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingApiary) return;
    setEditSubmitted(true);
    if (!editValidation.isValid) return;
    setLoading(true);
    try {
      await updateApiary(editingApiary.id, editForm);
      setMessage("Apiary updated successfully.");
      closeEdit();
      const data = await getApiaries();
      setApiaries(data);
      await fetchNearbyFields();
      const updated = data.find((a: Apiary) => a.id === editingApiary.id);
      if (updated) setSelectedApiary(updated);
    } catch {
      setMessage("Failed to update apiary.");
    }
    setLoading(false);
  };

  return (
    <div className="apiary-page">
      {/* Header */}
      <div className="apiary-header">
        <div>
          <div className="apiary-header-eyebrow">Smart Apiary</div>
          <h2>My Apiaries</h2>
        </div>
        <button onClick={openCreate} className="add-btn">
          + Add Apiary
        </button>
      </div>

      {/* Toast */}
      {message && (
        <div
          className={`apiary-toast ${
            isErrorMessage(message) ? "error" : "success-green"
          }`}
        >
          <span className="apiary-toast-dot"></span>
          {message}
        </div>
      )}

      {/* Three-column layout */}
      <div className="apiary-grid">
        {/* ── Left: Apiary list ── */}
        <aside className="apiary-list-card">
          <div className="apiary-list-card-header">
            <h3>Apiaries</h3>
            <span className="apiary-list-count-badge">{apiaries.length}</span>
          </div>
          <ul>
            {apiaries.length === 0 && (
              <div className="apiary-list-empty">No apiaries.</div>
            )}
            {apiaries.map((apiary) => (
              <li
                key={apiary.id}
                className={selectedApiary?.id === apiary.id ? "active" : ""}
                onClick={() => setSelectedApiary(apiary)}
              >
                <span className="apiary-list-icon">🐝</span>
                <div className="apiary-list-body">
                  <div className="apiary-list-name">{apiary.name}</div>
                  <div className="apiary-list-desc">{apiary.description}</div>
                  <div className="apiary-list-coords">
                    {Number(apiary.latitude).toFixed(3)},{" "}
                    {Number(apiary.longitude).toFixed(3)}
                  </div>
                </div>
              </li>
            ))}
          </ul>
        </aside>

        {/* ── Center: Map ── */}
        <section className="apiary-map">
          <div className="map-section">
            <MapErrorBoundary>
              <MapContainer
                center={[45.25, 19.85]}
                zoom={6}
                style={{ height: "100%", width: "100%" }}
              >
                <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
                {apiaries.map((apiary) => (
                  (typeof apiary.latitude === 'number' && typeof apiary.longitude === 'number') && (
                    <Marker
                      key={apiary.id}
                      position={[apiary.latitude, apiary.longitude]}
                      icon={orangeIcon}
                      eventHandlers={{ click: () => { setSelectedApiary(apiary); setSelectedField(null); } }}
                    >
                      <Popup>
                        <div className="apiary-popup">
                          <img
                            src={apiary.thumbnailUrl || apiary.imageUrl}
                            alt={apiary.name}
                          />
                          <div className="apiary-popup-name">{apiary.name}</div>
                          <p className="apiary-popup-desc">{apiary.description}</p>
                          <div className="apiary-popup-coords">
                            Lat: {apiary.latitude}, Lng: {apiary.longitude}
                          </div>
                        </div>
                      </Popup>
                    </Marker>
                  )
                ))}
                {/* Prikaz parcela sa kulturama u radijusu 5km */}
                {fieldsLoading ? null : (
                  nearbyFields.length > 0 ? (
                    nearbyFields.map((field) => (
                      (typeof field.latitude === 'number' && typeof field.longitude === 'number') && (
                        <Marker
                          key={field.fieldId}
                          position={[field.latitude, field.longitude]}
                          icon={greenIcon}
                          eventHandlers={{ click: () => { setSelectedField(field); setSelectedApiary(null); } }}
                        >
                          <Popup>
                            <div className="field-popup" style={{minWidth:220, padding:8}}>
                              <div className="field-popup-title" style={{fontWeight:'bold', fontSize:16, marginBottom:4}}>{field.fieldName}</div>
                              <div className="field-popup-owner" style={{marginBottom:4}}>
                                <span style={{fontWeight:'bold'}}>Owner:</span> {field.owner.firstName} {field.owner.lastName}<br/>
                                <span style={{fontWeight:'bold'}}>Email:</span> {field.owner.email}<br/>
                                <span style={{fontWeight:'bold'}}>Phone:</span> {field.owner.phone}
                              </div>
                              <div className="field-popup-crops" style={{marginTop:4}}>
                                <span style={{fontWeight:'bold'}}>Crops:</span>
                                {field.crops.length === 0 ? (
                                  <span style={{marginLeft:6, color:'#888'}}>No crops</span>
                                ) : (
                                  <ul style={{margin:0, paddingLeft:18}}>
                                    {field.crops.map((crop: NearbyField["crops"][0], idx: number) => (
                                      <li key={idx} style={{display:'flex', alignItems:'center', marginBottom:2}}>
                                        {crop.icon && <img src={crop.icon} alt={crop.name} style={{width:16, height:16, marginRight:4}}/>}
                                        <span style={{color:'#228B22', fontWeight:500}}>{crop.name}</span>
                                        <span style={{marginLeft:4, fontSize:12, color:'#666'}}>({crop.floweringPeriod})</span>
                                      </li>
                                    ))}
                                  </ul>
                                )}
                              </div>
                            </div>
                          </Popup>
                        </Marker>
                      )
                    ))
                  ) : (
                    <div className="map-fields-empty">No fields nearby.</div>
                  )
                )}
              </MapContainer>
            </MapErrorBoundary>
          </div>
        </section>

        {/* ── Right: Info panel ── */}
        <aside className="apiary-info-panel">
          <div className="apiary-info-header">
            <h3>{selectedApiary ? 'Apiary Info' : selectedField ? 'Field Info' : 'Info'}</h3>
            {(selectedApiary || selectedField) && (
              <button
                className="apiary-info-close"
                onClick={() => { setSelectedApiary(null); setSelectedField(null); }}
                aria-label="Close info panel"
              >
                ×
              </button>
            )}
          </div>

          {!selectedApiary && !selectedField ? (
            <div className="apiary-info-empty">
              Select an apiary or field from the map to see details.
            </div>
          ) : selectedApiary ? (
            <>
              {(selectedApiary.thumbnailUrl || selectedApiary.imageUrl) && (
                <img
                  className="apiary-info-image"
                  src={selectedApiary.thumbnailUrl || selectedApiary.imageUrl}
                  alt={selectedApiary.name}
                />
              )}
              <div className="apiary-info-section">
                <div className="apiary-info-row">
                  <span>Name</span>
                  <strong>{selectedApiary.name}</strong>
                </div>
                <div className="apiary-info-row">
                  <span>Latitude</span>
                  <strong>{selectedApiary.latitude}</strong>
                </div>
                <div className="apiary-info-row">
                  <span>Longitude</span>
                  <strong>{selectedApiary.longitude}</strong>
                </div>
                {selectedApiary.description && (
                  <div className="apiary-info-row apiary-info-row-block">
                    <span>Description</span>
                    <p>{selectedApiary.description}</p>
                  </div>
                )}
              </div>
              <button
                type="button"
                className="apiary-info-edit-btn"
                onClick={() => openEdit(selectedApiary)}
              >
                Edit Apiary
              </button>

              <HivesSection
                apiaryId={selectedApiary.id}
                onSuccess={(msg) => setMessage(msg)}
                onError={(msg) => setMessage(msg)}
              />
            </>
          ) : selectedField ? (
            <div className="field-info-panel" style={{padding:16}}>
              <div style={{fontWeight:'bold', fontSize:18, marginBottom:8}}>{selectedField.fieldName}</div>
              <div style={{marginBottom:6}}>
                <span style={{fontWeight:'bold'}}>Owner:</span> {selectedField.owner.firstName} {selectedField.owner.lastName}
              </div>
              <div style={{marginBottom:6}}>
                <span style={{fontWeight:'bold'}}>Email:</span> {selectedField.owner.email}
              </div>
              <div style={{marginBottom:6}}>
                <span style={{fontWeight:'bold'}}>Phone:</span> {selectedField.owner.phone}
              </div>
              <div style={{marginTop:10}}>
                <span style={{fontWeight:'bold'}}>Crops:</span>
                {selectedField.crops.length === 0 ? (
                  <span style={{marginLeft:6, color:'#888'}}>No crops</span>
                ) : (
                  <ul style={{margin:0, paddingLeft:18}}>
                    {selectedField.crops.map((crop, idx) => (
                      <li key={idx} style={{display:'flex', alignItems:'center', marginBottom:2}}>
                        {crop.icon && <img src={crop.icon} alt={crop.name} style={{width:16, height:16, marginRight:4}}/>}
                        <span style={{color:'#228B22', fontWeight:500}}>{crop.name}</span>
                        <span style={{marginLeft:4, fontSize:12, color:'#666'}}>({crop.floweringPeriod})</span>
                      </li>
                    ))}
                  </ul>
                )}
              </div>
            </div>
          ) : null}
        </aside>
      </div>

      {/* Add Apiary modal */}
      {showForm && (
        <div
          className="apiary-modal-backdrop"
          onClick={closeCreate}
        >
          <form
            className="apiary-form"
            onSubmit={handleSubmit}
            onClick={(e) => e.stopPropagation()}
          >
            <div className="apiary-form-header">
              <h3 className="apiary-form-title">Add New Apiary</h3>
              <button
                type="button"
                className="apiary-info-close"
                onClick={closeCreate}
                aria-label="Close form"
              >
                ×
              </button>
            </div>
            <div className="apiary-form-grid">
              <div className="apiary-form-group">
                <label htmlFor="apiary-name">Name</label>
                <input
                  id="apiary-name"
                  name="name"
                  placeholder="Apiary name"
                  value={form.name}
                  onChange={handleInputChange}
                />
                {createValidation.errors.name && shouldShowCreateError("name") && (
                  <span className="apiary-form-error">{createValidation.errors.name}</span>
                )}
              </div>
              <div className="apiary-form-group">
                <label htmlFor="apiary-description">Description</label>
                <textarea
                  id="apiary-description"
                  name="description"
                  placeholder="Description"
                  value={form.description}
                  onChange={handleInputChange}
                />
                {createValidation.errors.description && shouldShowCreateError("description") && (
                  <span className="apiary-form-error">{createValidation.errors.description}</span>
                )}
              </div>
              <div className="apiary-form-group">
                <label htmlFor="apiary-latitude">Latitude</label>
                <input
                  id="apiary-latitude"
                  name="latitude"
                  type="number"
                  min={-90}
                  max={90}
                  step="any"
                  placeholder="Latitude"
                  value={form.latitude}
                  onChange={handleInputChange}
                />
                {createValidation.errors.latitude && shouldShowCreateError("latitude") && (
                  <span className="apiary-form-error">{createValidation.errors.latitude}</span>
                )}
              </div>
              <div className="apiary-form-group">
                <label htmlFor="apiary-longitude">Longitude</label>
                <input
                  id="apiary-longitude"
                  name="longitude"
                  type="number"
                  min={-180}
                  max={180}
                  step="any"
                  placeholder="Longitude"
                  value={form.longitude}
                  onChange={handleInputChange}
                />
                {createValidation.errors.longitude && shouldShowCreateError("longitude") && (
                  <span className="apiary-form-error">{createValidation.errors.longitude}</span>
                )}
              </div>
              <div className="apiary-form-group apiary-form-file-group">
                <label htmlFor="apiary-file">Image</label>
                <input
                  id="apiary-file"
                  name="file"
                  type="file"
                  accept="image/*"
                  onChange={handleInputChange}
                  required
                />
                <label htmlFor="apiary-file" className="apiary-form-file-label">
                  Choose Image
                </label>
                {imagePreview && (
                  <img
                    src={imagePreview}
                    alt="Preview"
                    className="apiary-form-image-preview"
                  />
                )}
              </div>
            </div>
            <button type="submit" disabled={loading}>
              {loading ? "Saving..." : "Save"}
            </button>
          </form>
        </div>
      )}

      {/* Edit Apiary modal */}
      {editingApiary && (
        <div className="apiary-modal-backdrop" onClick={closeEdit}>
          <form
            className="apiary-form"
            onSubmit={handleEditSubmit}
            onClick={(e) => e.stopPropagation()}
          >
            <div className="apiary-form-header">
              <h3 className="apiary-form-title">Edit Apiary</h3>
              <button
                type="button"
                className="apiary-info-close"
                onClick={closeEdit}
                aria-label="Close form"
              >
                ×
              </button>
            </div>
            <div className="apiary-form-grid">
              <div className="apiary-form-group">
                <label htmlFor="edit-apiary-name">Name</label>
                <input
                  id="edit-apiary-name"
                  name="name"
                  placeholder="Apiary name"
                  value={editForm.name}
                  onChange={handleEditChange}
                />
                {editValidation.errors.name && shouldShowEditError("name") && (
                  <span className="apiary-form-error">{editValidation.errors.name}</span>
                )}
              </div>
              <div className="apiary-form-group">
                <label htmlFor="edit-apiary-description">Description</label>
                <textarea
                  id="edit-apiary-description"
                  name="description"
                  placeholder="Description"
                  value={editForm.description}
                  onChange={handleEditChange}
                />
                {editValidation.errors.description && shouldShowEditError("description") && (
                  <span className="apiary-form-error">{editValidation.errors.description}</span>
                )}
              </div>
              <div className="apiary-form-group">
                <label htmlFor="edit-apiary-latitude">Latitude</label>
                <input
                  id="edit-apiary-latitude"
                  name="latitude"
                  type="number"
                  min={-90}
                  max={90}
                  step="any"
                  placeholder="Latitude"
                  value={editForm.latitude}
                  onChange={handleEditChange}
                />
                {editValidation.errors.latitude && shouldShowEditError("latitude") && (
                  <span className="apiary-form-error">{editValidation.errors.latitude}</span>
                )}
              </div>
              <div className="apiary-form-group">
                <label htmlFor="edit-apiary-longitude">Longitude</label>
                <input
                  id="edit-apiary-longitude"
                  name="longitude"
                  type="number"
                  min={-180}
                  max={180}
                  step="any"
                  placeholder="Longitude"
                  value={editForm.longitude}
                  onChange={handleEditChange}
                />
                {editValidation.errors.longitude && shouldShowEditError("longitude") && (
                  <span className="apiary-form-error">{editValidation.errors.longitude}</span>
                )}
              </div>
              <div className="apiary-form-group apiary-form-file-group">
                <label htmlFor="edit-apiary-file">Image (optional)</label>
                <input
                  id="edit-apiary-file"
                  name="file"
                  type="file"
                  accept="image/*"
                  onChange={handleEditChange}
                />
                <label htmlFor="edit-apiary-file" className="apiary-form-file-label">
                  Choose New Image
                </label>
                {(editImagePreview ||
                  editingApiary.thumbnailUrl ||
                  editingApiary.imageUrl) && (
                  <img
                    src={
                      editImagePreview ||
                      editingApiary.thumbnailUrl ||
                      editingApiary.imageUrl
                    }
                    alt="Preview"
                    className="apiary-form-image-preview"
                  />
                )}
              </div>
            </div>
            <button type="submit" disabled={loading}>
              {loading ? "Saving..." : "Save Changes"}
            </button>
          </form>
        </div>
      )}
    </div>
  );
};

export default ApiaryPage;
