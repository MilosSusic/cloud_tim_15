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
import HiveForm from "./HiveForm";
import HiveListItem from "./HiveListItem";
import "../layout/HivesSection.css";
import { registerScale } from "../../scales/api/scaleApi";
import RegisterScaleModal from "../../scales/components/RegisterScaleModal";

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
  const [scaleHive, setScaleHive] = useState<Hive | null>(null);
  const [scaleLoading, setScaleLoading] = useState(false);
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

  const handleRegisterScale = async (serialNumber: string) => {
    if (!scaleHive) return;
    setScaleLoading(true);
    try {
      await registerScale(scaleHive.id, serialNumber);
      onSuccess?.(
        `Scale ${serialNumber} successfully attached to hive ${scaleHive.label}.`,
      );
      setScaleHive(null);
      await loadHives();
    } catch (err: any) {
      throw err;
    } finally {
      setScaleLoading(false);
    }
  };

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
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
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
      onError?.(
        editingHive ? "Failed to update hive." : "Failed to create hive.",
      );
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
          <HiveListItem
            key={hive.id}
            hive={hive}
            onEdit={openEdit}
            onDelete={handleDelete}
            onOpenLog={setLogHive}
            onOpenScale={setScaleHive}
          />
        ))}
      </ul>

      {showForm && (
        <HiveForm
          editingHive={editingHive}
          form={form}
          loading={loading}
          errors={hiveValidation.errors}
          shouldShowError={shouldShowError}
          onChange={handleChange}
          onSubmit={handleSubmit}
          onClose={closeForm}
        />
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

      {scaleHive && (
        <RegisterScaleModal
          hiveLabel={scaleHive.label}
          loading={scaleLoading}
          onConfirm={handleRegisterScale}
          onClose={() => setScaleHive(null)}
        />
      )}
    </div>
  );
};

export default HivesSection;
