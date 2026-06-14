import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useApiaryValidation } from "../hooks/useApiaryValidation";
import { getApiaries, createApiary, updateApiary } from "../api/apiaryApi";
import { getNearbyFieldsWithCrops } from "../api/nearbyFieldsApi";
import ApiaryForm from "../components/ApiaryForm";
import ApiaryListPanel from "../components/ApiaryListPanel";
import ApiaryMap from "../components/ApiaryMap";
import ApiaryInfoPanel from "../components/ApiaryInfoPanel";
import "../layout/ApiaryPage.css";
import type { Apiary } from "../types/Apiary";
import type { NearbyField } from "../types/NearbyField";
import { useAuth } from "../../../shared/auth/AuthContext";

const ApiaryPage: React.FC = () => {
  const navigate = useNavigate();
  const { logout } = useAuth();
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
  const [createTouched, setCreateTouched] = useState(
    createEmptyApiaryTouched(),
  );
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
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
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
    if (!form.file) {
      setMessage("Image is required.");
      return;
    }
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
      const message =
        err instanceof Error ? err.message : "Failed to create apiary.";
      setMessage(message);
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
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
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

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="apiary-page">
      <div className="apiary-header">
        <div>
          <div className="apiary-header-eyebrow">Smart Apiary</div>
          <h2>My Apiaries</h2>
        </div>
        <div className="apiary-header-actions">
          <button
            onClick={() => navigate("/beekeeper/analytics")}
            className="analytics-nav-btn"
          >
            📊 Analytics
          </button>
          <button onClick={openCreate} className="add-btn">
            + Add Apiary
          </button>
          <button onClick={handleLogout} className="logout-btn">
            ⎋ Logout
          </button>
        </div>
      </div>

      {message && (
        <div
          className={`apiary-toast ${isErrorMessage(message) ? "error" : "success-green"}`}
        >
          <span className="apiary-toast-dot"></span>
          {message}
        </div>
      )}

      <div className="apiary-grid">
        <ApiaryListPanel
          apiaries={apiaries}
          selectedId={selectedApiary?.id}
          loading={loading}
          onSelect={(apiary) => {
            setSelectedApiary(apiary);
            setSelectedField(null);
          }}
        />

        <ApiaryMap
          apiaries={apiaries}
          nearbyFields={nearbyFields}
          fieldsLoading={fieldsLoading}
          onSelectApiary={(apiary) => {
            setSelectedApiary(apiary);
            setSelectedField(null);
          }}
          onSelectField={(field) => {
            setSelectedField(field);
            setSelectedApiary(null);
          }}
        />

        <ApiaryInfoPanel
          selectedApiary={selectedApiary}
          selectedField={selectedField}
          onClose={() => {
            setSelectedApiary(null);
            setSelectedField(null);
          }}
          onEdit={openEdit}
          onMessage={setMessage}
        />
      </div>

      {showForm && (
        <ApiaryForm
          title="Add New Apiary"
          submitLabel="Save"
          form={form}
          imagePreview={imagePreview}
          loading={loading}
          errors={createValidation.errors}
          shouldShowError={shouldShowCreateError}
          onChange={handleInputChange}
          onSubmit={handleSubmit}
          onClose={closeCreate}
          imageRequired
        />
      )}

      {editingApiary && (
        <ApiaryForm
          title="Edit Apiary"
          submitLabel="Save Changes"
          form={editForm}
          imagePreview={editImagePreview}
          existingImageUrl={
            editingApiary.thumbnailUrl || editingApiary.imageUrl
          }
          loading={loading}
          errors={editValidation.errors}
          shouldShowError={shouldShowEditError}
          onChange={handleEditChange}
          onSubmit={handleEditSubmit}
          onClose={closeEdit}
        />
      )}
    </div>
  );
};

export default ApiaryPage;
