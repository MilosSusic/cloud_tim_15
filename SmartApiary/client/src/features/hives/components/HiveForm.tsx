import React from "react";
import type { Hive, HiveFormState } from "../types/Hive";

interface HiveFormErrors {
  label?: string;
  type?: string;
  color?: string;
  queenAge?: string;
  note?: string;
}

interface Props {
  editingHive: Hive | null;
  form: HiveFormState;
  loading: boolean;
  errors: HiveFormErrors;
  shouldShowError: (field: keyof HiveFormErrors) => boolean;
  onChange: (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => void;
  onSubmit: (e: React.FormEvent) => void;
  onClose: () => void;
}

const HiveForm: React.FC<Props> = ({ editingHive, form, loading, errors, shouldShowError, onChange, onSubmit, onClose }) => (
  <div className="apiary-modal-backdrop" onClick={onClose}>
    <form className="apiary-form" onSubmit={onSubmit} onClick={(e) => e.stopPropagation()}>
      <div className="apiary-form-header">
        <h3 className="apiary-form-title">{editingHive ? "Edit Hive" : "Add Hive"}</h3>
        <button type="button" className="apiary-info-close" onClick={onClose} aria-label="Close form">×</button>
      </div>
      <div className="apiary-form-grid">
        <div className="apiary-form-group">
          <label htmlFor="hive-label">Label</label>
          <input id="hive-label" name="label" placeholder="K1" value={form.label} onChange={onChange} />
          {errors.label && shouldShowError("label") && <span className="apiary-form-error">{errors.label}</span>}
        </div>
        <div className="apiary-form-group">
          <label htmlFor="hive-type">Type</label>
          <input id="hive-type" name="type" placeholder="LR / DB / Langstroth" value={form.type} onChange={onChange} />
          {errors.type && shouldShowError("type") && <span className="apiary-form-error">{errors.type}</span>}
        </div>
        <div className="apiary-form-group">
          <label htmlFor="hive-color">Color</label>
          <input id="hive-color" name="color" placeholder="Yellow" value={form.color} onChange={onChange} />
          {errors.color && shouldShowError("color") && <span className="apiary-form-error">{errors.color}</span>}
        </div>
        <div className="apiary-form-group">
          <label htmlFor="hive-queenAge">Queen age (years)</label>
          <input id="hive-queenAge" name="queenAge" type="number" min={0} max={10} value={form.queenAge} onChange={onChange} />
          {errors.queenAge && shouldShowError("queenAge") && <span className="apiary-form-error">{errors.queenAge}</span>}
        </div>
        <div className="apiary-form-group apiary-form-file-group">
          <label htmlFor="hive-note">Note</label>
          <textarea id="hive-note" name="note" placeholder="Optional note" value={form.note} onChange={onChange} />
          {errors.note && shouldShowError("note") && <span className="apiary-form-error">{errors.note}</span>}
        </div>
      </div>
      <button type="submit" disabled={loading}>
        {loading ? "Saving..." : editingHive ? "Save Changes" : "Save"}
      </button>
    </form>
  </div>
);

export default HiveForm;
