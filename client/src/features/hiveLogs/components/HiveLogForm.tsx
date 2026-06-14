import React from "react";
import type { HiveLogFormState } from "../types/HiveLogEntry";

interface HiveLogFormErrors {
  reviewDate?: string;
  reviewType?: string;
  bottomBoardColor?: string;
  honeyFrames?: string;
  honeyAmountKg?: string;
  broodFrames?: string;
  note?: string;
}

interface Props {
  editingId: string | null;
  form: HiveLogFormState;
  loading: boolean;
  errors: HiveLogFormErrors;
  maxDatetime: string;
  shouldShowError: (field: keyof HiveLogFormErrors) => boolean;
  onChange: (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => void;
  onSubmit: (e: React.FormEvent) => void;
  onClose: () => void;
}

const HiveLogForm: React.FC<Props> = ({
  editingId, form, loading, errors, maxDatetime, shouldShowError, onChange, onSubmit, onClose,
}) => (
  <div className="apiary-modal-backdrop hivelog-form-backdrop" onClick={onClose}>
    <form className="apiary-form" onSubmit={onSubmit} onClick={(e) => e.stopPropagation()}>
      <div className="apiary-form-header">
        <h3 className="apiary-form-title">{editingId ? "Edit log entry" : "New log entry"}</h3>
        <button type="button" className="apiary-info-close" onClick={onClose} aria-label="Close">×</button>
      </div>
      <div className="apiary-form-grid">
        <div className="apiary-form-group">
          <label htmlFor="hl-date">Review date</label>
          <input id="hl-date" name="reviewDate" type="datetime-local" max={maxDatetime} value={form.reviewDate} onChange={onChange} />
          {errors.reviewDate && shouldShowError("reviewDate") && <span className="apiary-form-error">{errors.reviewDate}</span>}
        </div>
        <div className="apiary-form-group">
          <label htmlFor="hl-type">Review type</label>
          <input id="hl-type" name="reviewType" value={form.reviewType} onChange={onChange} maxLength={100} />
          {errors.reviewType && shouldShowError("reviewType") && <span className="apiary-form-error">{errors.reviewType}</span>}
        </div>
        <div className="apiary-form-group">
          <label htmlFor="hl-color">Bottom board color</label>
          <input id="hl-color" name="bottomBoardColor" value={form.bottomBoardColor} onChange={onChange} maxLength={50} />
          {errors.bottomBoardColor && shouldShowError("bottomBoardColor") && <span className="apiary-form-error">{errors.bottomBoardColor}</span>}
        </div>
        <div className="apiary-form-group">
          <label htmlFor="hl-honey-frames">Honey frames</label>
          <input id="hl-honey-frames" name="honeyFrames" type="number" min={0} value={form.honeyFrames} onChange={onChange} />
          {errors.honeyFrames && shouldShowError("honeyFrames") && <span className="apiary-form-error">{errors.honeyFrames}</span>}
        </div>
        <div className="apiary-form-group">
          <label htmlFor="hl-honey-kg">Honey amount (kg)</label>
          <input id="hl-honey-kg" name="honeyAmountKg" type="number" min={0} step="0.1" value={form.honeyAmountKg} onChange={onChange} />
          {errors.honeyAmountKg && shouldShowError("honeyAmountKg") && <span className="apiary-form-error">{errors.honeyAmountKg}</span>}
        </div>
        <div className="apiary-form-group">
          <label htmlFor="hl-brood">Brood frames</label>
          <input id="hl-brood" name="broodFrames" type="number" min={0} value={form.broodFrames} onChange={onChange} />
          {errors.broodFrames && shouldShowError("broodFrames") && <span className="apiary-form-error">{errors.broodFrames}</span>}
        </div>
        <div className="apiary-form-group">
          <label className="hivelog-checkbox-label">
            <input name="queenPresent" type="checkbox" checked={form.queenPresent} onChange={onChange} />
            Queen present
          </label>
        </div>
        <div className="apiary-form-group">
          <label htmlFor="hl-note">Note</label>
          <textarea id="hl-note" name="note" value={form.note} onChange={onChange} maxLength={512} placeholder="Free text..." />
          {errors.note && shouldShowError("note") && <span className="apiary-form-error">{errors.note}</span>}
        </div>
      </div>
      <div className="apiary-form-actions">
        <button type="button" className="apiary-form-cancel" onClick={onClose}>Cancel</button>
        <button type="submit" className="add-btn" disabled={loading}>{editingId ? "Save" : "Add"}</button>
      </div>
    </form>
  </div>
);

export default HiveLogForm;
