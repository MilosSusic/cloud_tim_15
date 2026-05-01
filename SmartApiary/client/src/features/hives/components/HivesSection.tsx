import React, { useEffect, useState } from "react";
import {
  getHivesByApiary,
  createHive,
  updateHive,
  deleteHive,
} from "../api/hiveApi";
import type { Hive, HiveFormState } from "../types/Hive";
import { useHiveValidation } from "../hooks/useHiveValidation";
import HiveLogModal from "../../hiveLogs/components/HiveLogModal";
import "../layout/HivesSection.css";

interface Props {
  apiaryId: string;
  onError?: (msg: string) => void;
  onSuccess?: (msg: string) => void;
}

const emptyForm: HiveFormState = {
  label: "",
  type: "LR",
  color: "",
  queenAge: "0",
  note: "",
};

const HivesSection: React.FC<Props> = ({ apiaryId, onError, onSuccess }) => {
  const [hives, setHives] = useState<Hive[]>([]);
  const [loading, setLoading] = useState(false);
  const [showForm, setShowForm] = useState(false);
  const [editingHive, setEditingHive] = useState<Hive | null>(null);
  const [form, setForm] = useState<HiveFormState>(emptyForm);
  const [logHive, setLogHive] = useState<Hive | null>(null);
  const createEmptyHiveTouched = () => ({
    label: false,
    type: false,
    color: false,
    queenAge: false,
    note: false,
  });
  const [touched, setTouched] = useState(createEmptyHiveTouched());
  const [submitted, setSubmitted] = useState(false);

  const hiveValidation = useHiveValidation(form);
  const shouldShowError = (field: keyof typeof touched) =>
    submitted || touched[field];

  const loadHives = async () => {
    setLoading(true);
    try {
      const data = await getHivesByApiary(apiaryId);
      setHives(data);
    } catch {
      onError?.("Failed to load hives.");
    }
    setLoading(false);
  };

  useEffect(() => {
    loadHives();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [apiaryId]);

  const openCreate = () => {
    setEditingHive(null);
    setForm(emptyForm);
    setTouched(createEmptyHiveTouched());
    setSubmitted(false);
    setShowForm(true);
  };

  const openEdit = (hive: Hive) => {
    setEditingHive(hive);
    setForm({
      label: hive.label,
      type: hive.type,
      color: hive.color,
      queenAge: String(hive.queenAge),
      note: hive.note ?? "",
    });
    setTouched(createEmptyHiveTouched());
    setSubmitted(false);
    setShowForm(true);
  };

  const closeForm = () => {
    setShowForm(false);
    setEditingHive(null);
    setForm(emptyForm);
    setTouched(createEmptyHiveTouched());
    setSubmitted(false);
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    setForm({ ...form, [name]: value });
    if (
      name === "label" ||
      name === "type" ||
      name === "color" ||
      name === "queenAge" ||
      name === "note"
    ) {
      setTouched({ ...touched, [name]: true });
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitted(true);
    if (!hiveValidation.isValid) return;
    setLoading(true);
    try {
      if (editingHive) {
        await updateHive(editingHive.id, {
          id: editingHive.id,
          label: form.label,
          type: form.type,
          color: form.color,
          queenAge: Number(form.queenAge) || 0,
          note: form.note || null,
        });
        onSuccess?.("Hive updated successfully.");
      } else {
        await createHive({
          apiaryId,
          label: form.label,
          type: form.type,
          color: form.color,
          queenAge: Number(form.queenAge) || 0,
          note: form.note || null,
        });
        onSuccess?.("Hive created successfully.");
      }
      closeForm();
      await loadHives();
    } catch {
      onError?.(editingHive ? "Failed to update hive." : "Failed to create hive.");
    }
    setLoading(false);
  };

  const handleDelete = async (hive: Hive) => {
    if (!window.confirm(`Delete hive "${hive.label}"?`)) return;
    setLoading(true);
    try {
      await deleteHive(hive.id);
      onSuccess?.("Hive deleted.");
      await loadHives();
    } catch {
      onError?.("Failed to delete hive.");
    }
    setLoading(false);
  };

  return (
    <div className="hives-section">
      <div className="hives-section-header">
        <h4>
          Hives <span className="hives-count">({hives.length})</span>
        </h4>
        <button type="button" className="hives-add-btn" onClick={openCreate}>
          + Add
        </button>
      </div>

      {hives.length === 0 && !loading && (
        <div className="hives-empty">No hives yet.</div>
      )}

      <ul className="hives-list">
        {hives.map((hive) => (
          <li key={hive.id} className="hives-list-item">
            <div className="hives-list-main">
              <div className="hives-list-label">{hive.label}</div>
              <div className="hives-list-meta">
                {hive.type} · {hive.color} · queen {hive.queenAge}y
              </div>
              {hive.note && <div className="hives-list-note">{hive.note}</div>}
            </div>
            <div className="hives-list-actions">
              <button
                type="button"
                className="hives-icon-btn"
                onClick={() => setLogHive(hive)}
                aria-label="Open hive log"
                title="Log"
              >
                📓
              </button>
              <button
                type="button"
                className="hives-icon-btn"
                onClick={() => openEdit(hive)}
                aria-label="Edit hive"
                title="Edit"
              >
                ✏️
              </button>
              <button
                type="button"
                className="hives-icon-btn hives-icon-btn-danger"
                onClick={() => handleDelete(hive)}
                aria-label="Delete hive"
                title="Delete"
              >
                🗑️
              </button>
            </div>
          </li>
        ))}
      </ul>

      {showForm && (
        <div className="apiary-modal-backdrop" onClick={closeForm}>
          <form
            className="apiary-form"
            onSubmit={handleSubmit}
            onClick={(e) => e.stopPropagation()}
          >
            <div className="apiary-form-header">
              <h3 className="apiary-form-title">
                {editingHive ? "Edit Hive" : "Add Hive"}
              </h3>
              <button
                type="button"
                className="apiary-info-close"
                onClick={closeForm}
                aria-label="Close form"
              >
                ×
              </button>
            </div>
            <div className="apiary-form-grid">
              <div className="apiary-form-group">
                <label htmlFor="hive-label">Label</label>
                <input
                  id="hive-label"
                  name="label"
                  placeholder="K1"
                  value={form.label}
                  onChange={handleChange}
                />
                {hiveValidation.errors.label && shouldShowError("label") && (
                  <span className="apiary-form-error">{hiveValidation.errors.label}</span>
                )}
              </div>
              <div className="apiary-form-group">
                <label htmlFor="hive-type">Type</label>
                <input
                  id="hive-type"
                  name="type"
                  placeholder="LR / DB / Položka"
                  value={form.type}
                  onChange={handleChange}
                />
                {hiveValidation.errors.type && shouldShowError("type") && (
                  <span className="apiary-form-error">{hiveValidation.errors.type}</span>
                )}
              </div>
              <div className="apiary-form-group">
                <label htmlFor="hive-color">Color</label>
                <input
                  id="hive-color"
                  name="color"
                  placeholder="Yellow"
                  value={form.color}
                  onChange={handleChange}
                />
                {hiveValidation.errors.color && shouldShowError("color") && (
                  <span className="apiary-form-error">{hiveValidation.errors.color}</span>
                )}
              </div>
              <div className="apiary-form-group">
                <label htmlFor="hive-queenAge">Queen age (years)</label>
                <input
                  id="hive-queenAge"
                  name="queenAge"
                  type="number"
                  min={0}
                  max={10}
                  value={form.queenAge}
                  onChange={handleChange}
                />
                {hiveValidation.errors.queenAge && shouldShowError("queenAge") && (
                  <span className="apiary-form-error">{hiveValidation.errors.queenAge}</span>
                )}
              </div>
              <div className="apiary-form-group apiary-form-file-group">
                <label htmlFor="hive-note">Note</label>
                <textarea
                  id="hive-note"
                  name="note"
                  placeholder="Optional note"
                  value={form.note}
                  onChange={handleChange}
                />
                {hiveValidation.errors.note && shouldShowError("note") && (
                  <span className="apiary-form-error">{hiveValidation.errors.note}</span>
                )}
              </div>
            </div>
            <button type="submit" disabled={loading}>
              {loading ? "Saving..." : editingHive ? "Save Changes" : "Create"}
            </button>
          </form>
        </div>
      )}

      {logHive && (
        <HiveLogModal
          hiveId={logHive.id}
          hiveLabel={logHive.label}
          onClose={() => setLogHive(null)}
          onError={onError}
          onSuccess={onSuccess}
        />
      )}
    </div>
  );
};

export default HivesSection;
