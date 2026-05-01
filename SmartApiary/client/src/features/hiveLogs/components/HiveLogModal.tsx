import React, { useEffect, useState } from "react";
import {
  getLogsByHive,
  createLogEntry,
  updateLogEntry,
  deleteLogEntry,
} from "../api/hiveLogApi";
import type {
  HiveLogEntryDto,
  HiveLogFormState,
} from "../types/HiveLogEntry";
import { useHiveLogValidation } from "../hooks/useHiveLogValidation";
import "../layout/HiveLogModal.css";

interface Props {
  hiveId: string;
  hiveLabel: string;
  onClose: () => void;
  onError?: (msg: string) => void;
  onSuccess?: (msg: string) => void;
}

const PAGE_SIZE = 5;

const toDatetimeLocal = (iso: string) => {
  const d = new Date(iso);
  const off = d.getTimezoneOffset();
  const local = new Date(d.getTime() - off * 60000);
  return local.toISOString().slice(0, 16);
};

const nowDatetimeLocal = () => toDatetimeLocal(new Date().toISOString());
const maxAllowedDatetimeLocal = () =>
  toDatetimeLocal(new Date(Date.now() + 5 * 60 * 1000).toISOString());

const emptyForm = (): HiveLogFormState => ({
  reviewDate: nowDatetimeLocal(),
  reviewType: "Routine inspection",
  bottomBoardColor: "",
  honeyFrames: "0",
  honeyAmountKg: "0",
  broodFrames: "0",
  queenPresent: true,
  note: "",
});

const HiveLogModal: React.FC<Props> = ({
  hiveId,
  hiveLabel,
  onClose,
  onError,
  onSuccess,
}) => {
  const [items, setItems] = useState<HiveLogEntryDto[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(false);

  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState<HiveLogFormState>(emptyForm());
  const createEmptyTouched = () => ({
    reviewDate: false,
    reviewType: false,
    bottomBoardColor: false,
    honeyFrames: false,
    honeyAmountKg: false,
    broodFrames: false,
    note: false,
  });
  const [touched, setTouched] = useState(createEmptyTouched());
  const [submitted, setSubmitted] = useState(false);

  const validation = useHiveLogValidation(form, hiveId, editingId);
  const shouldShowError = (field: keyof typeof touched) =>
    submitted || touched[field];

  const loadPage = async (p: number) => {
    setLoading(true);
    try {
      const data = await getLogsByHive(hiveId, p, PAGE_SIZE);
      setItems(data.items);
      setPage(data.page);
      setTotalPages(Math.max(1, data.totalPages));
      setTotalCount(data.totalCount);
    } catch (e) {
      onError?.((e as Error).message);
    }
    setLoading(false);
  };

  useEffect(() => {
    loadPage(1);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [hiveId]);

  const openCreate = () => {
    setEditingId(null);
    setForm(emptyForm());
    setTouched(createEmptyTouched());
    setSubmitted(false);
    setShowForm(true);
  };

  const openEdit = (entry: HiveLogEntryDto) => {
    setEditingId(entry.id);
    setForm({
      reviewDate: toDatetimeLocal(entry.reviewDate),
      reviewType: entry.reviewType,
      bottomBoardColor: entry.bottomBoardColor,
      honeyFrames: String(entry.honeyFrames),
      honeyAmountKg: String(entry.honeyAmountKg),
      broodFrames: String(entry.broodFrames),
      queenPresent: entry.queenPresent,
      note: entry.note ?? "",
    });
    setTouched(createEmptyTouched());
    setSubmitted(false);
    setShowForm(true);
  };

  const closeForm = () => {
    setShowForm(false);
    setEditingId(null);
    setForm(emptyForm());
    setTouched(createEmptyTouched());
    setSubmitted(false);
  };

  const handleChange = (
    e: React.ChangeEvent<
      HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
    >
  ) => {
    const { name, value, type } = e.target;
    if (type === "checkbox") {
      setForm({
        ...form,
        [name]: (e.target as HTMLInputElement).checked,
      });
    } else {
      setForm({ ...form, [name]: value });
      if (
        name === "reviewDate" ||
        name === "reviewType" ||
        name === "bottomBoardColor" ||
        name === "honeyFrames" ||
        name === "honeyAmountKg" ||
        name === "broodFrames" ||
        name === "note"
      ) {
        setTouched({ ...touched, [name]: true });
      }
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitted(true);
    if (!validation.isValid) {
      if (validation.errors.form) {
        onError?.(validation.errors.form);
      }
      return;
    }
    setLoading(true);
    try {
      const isoDate = new Date(form.reviewDate).toISOString();
      const basePayload = {
        reviewDate: isoDate,
        reviewType: form.reviewType,
        bottomBoardColor: form.bottomBoardColor,
        honeyFrames: Number(form.honeyFrames) || 0,
        honeyAmountKg: Number(form.honeyAmountKg) || 0,
        broodFrames: Number(form.broodFrames) || 0,
        queenPresent: form.queenPresent,
        note: form.note || null,
      };
      if (editingId) {
        await updateLogEntry(editingId, { id: editingId, ...basePayload });
        onSuccess?.("Log entry updated.");
      } else {
        await createLogEntry({ hiveId, ...basePayload });
        onSuccess?.("Log entry added.");
      }
      closeForm();
      await loadPage(editingId ? page : 1);
    } catch (err) {
      onError?.((err as Error).message);
    }
    setLoading(false);
  };

  const handleDelete = async (entry: HiveLogEntryDto) => {
    if (!window.confirm("Delete this log entry?")) return;
    setLoading(true);
    try {
      await deleteLogEntry(entry.id);
      onSuccess?.("Log entry deleted.");
      const nextPage =
        items.length === 1 && page > 1 ? page - 1 : page;
      await loadPage(nextPage);
    } catch (err) {
      onError?.((err as Error).message);
    }
    setLoading(false);
  };

  const renderPagination = () => {
    if (totalPages <= 1) return null;
    const pages: number[] = [];
    for (let i = 1; i <= totalPages; i++) pages.push(i);
    return (
      <div className="hivelog-pagination">
        <button
          type="button"
          className="hivelog-page-btn"
          disabled={page <= 1 || loading}
          onClick={() => loadPage(page - 1)}
        >
          ‹ Prev
        </button>
        {pages.map((p) => (
          <button
            key={p}
            type="button"
            className={`hivelog-page-btn ${p === page ? "active" : ""}`}
            disabled={loading}
            onClick={() => loadPage(p)}
          >
            {p}
          </button>
        ))}
        <button
          type="button"
          className="hivelog-page-btn"
          disabled={page >= totalPages || loading}
          onClick={() => loadPage(page + 1)}
        >
          Next ›
        </button>
      </div>
    );
  };

  return (
    <div className="apiary-modal-backdrop" onClick={onClose}>
      <div
        className="hivelog-modal"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="hivelog-header">
          <div>
            <div className="hivelog-eyebrow">Hive log</div>
            <h3 className="hivelog-title">{hiveLabel}</h3>
            <div className="hivelog-subtitle">
              {totalCount} {totalCount === 1 ? "entry" : "entries"}
            </div>
          </div>
          <div className="hivelog-header-actions">
            <button
              type="button"
              className="hivelog-add-btn"
              onClick={openCreate}
            >
              + New entry
            </button>
            <button
              type="button"
              className="apiary-info-close"
              onClick={onClose}
              aria-label="Close"
            >
              ×
            </button>
          </div>
        </div>

        <div className="hivelog-body">
          {loading && items.length === 0 && (
            <div className="hivelog-empty">Loading...</div>
          )}
          {!loading && items.length === 0 && (
            <div className="hivelog-empty">No log entries for this hive yet.</div>
          )}
          <ul className="hivelog-list">
            {items.map((entry) => (
              <li key={entry.id} className="hivelog-item">
                <div className="hivelog-item-head">
                  <div className="hivelog-item-date">
                    {new Date(entry.reviewDate).toLocaleString()}
                  </div>
                  <div className="hivelog-item-actions">
                    <button
                      type="button"
                      className="hives-icon-btn"
                      onClick={() => openEdit(entry)}
                      title="Edit"
                    >
                      ✏️
                    </button>
                    <button
                      type="button"
                      className="hives-icon-btn hives-icon-btn-danger"
                      onClick={() => handleDelete(entry)}
                      title="Delete"
                    >
                      🗑️
                    </button>
                  </div>
                </div>
                <div className="hivelog-item-type">{entry.reviewType}</div>
                <div className="hivelog-item-stats">
                  <span>
                    Honey: <strong>{entry.honeyAmountKg} kg</strong> (
                    {entry.honeyFrames} frames)
                  </span>
                  <span>
                    Brood: <strong>{entry.broodFrames}</strong> frames
                  </span>
                  <span>
                    Queen:{" "}
                    <strong>{entry.queenPresent ? "Yes" : "No"}</strong>
                  </span>
                  {entry.bottomBoardColor && (
                    <span>
                      Bottom board: <strong>{entry.bottomBoardColor}</strong>
                    </span>
                  )}
                </div>
                {entry.note && (
                  <div className="hivelog-item-note">{entry.note}</div>
                )}
              </li>
            ))}
          </ul>
          {renderPagination()}
        </div>

        {showForm && (
          <div
            className="apiary-modal-backdrop hivelog-form-backdrop"
            onClick={closeForm}
          >
            <form
              className="apiary-form"
              onSubmit={handleSubmit}
              onClick={(e) => e.stopPropagation()}
            >
              <div className="apiary-form-header">
                <h3 className="apiary-form-title">
                  {editingId ? "Edit log entry" : "New log entry"}
                </h3>
                <button
                  type="button"
                  className="apiary-info-close"
                  onClick={closeForm}
                  aria-label="Close"
                >
                  ×
                </button>
              </div>
              <div className="apiary-form-grid">
                <div className="apiary-form-group">
                  <label htmlFor="hl-date">Review date</label>
                  <input
                    id="hl-date"
                    name="reviewDate"
                    type="datetime-local"
                    max={maxAllowedDatetimeLocal()}
                    value={form.reviewDate}
                    onChange={handleChange}
                  />
                  {validation.errors.reviewDate && shouldShowError("reviewDate") && (
                    <span className="apiary-form-error">{validation.errors.reviewDate}</span>
                  )}
                </div>
                <div className="apiary-form-group">
                  <label htmlFor="hl-type">Review type</label>
                  <input
                    id="hl-type"
                    name="reviewType"
                    value={form.reviewType}
                    onChange={handleChange}
                    maxLength={100}
                  />
                  {validation.errors.reviewType && shouldShowError("reviewType") && (
                    <span className="apiary-form-error">{validation.errors.reviewType}</span>
                  )}
                </div>
                <div className="apiary-form-group">
                  <label htmlFor="hl-color">Bottom board color</label>
                  <input
                    id="hl-color"
                    name="bottomBoardColor"
                    value={form.bottomBoardColor}
                    onChange={handleChange}
                    maxLength={50}
                  />
                  {validation.errors.bottomBoardColor &&
                    shouldShowError("bottomBoardColor") && (
                      <span className="apiary-form-error">{validation.errors.bottomBoardColor}</span>
                    )}
                </div>
                <div className="apiary-form-group">
                  <label htmlFor="hl-honey-frames">Honey frames</label>
                  <input
                    id="hl-honey-frames"
                    name="honeyFrames"
                    type="number"
                    min={0}
                    value={form.honeyFrames}
                    onChange={handleChange}
                  />
                  {validation.errors.honeyFrames && shouldShowError("honeyFrames") && (
                    <span className="apiary-form-error">{validation.errors.honeyFrames}</span>
                  )}
                </div>
                <div className="apiary-form-group">
                  <label htmlFor="hl-honey-kg">Honey amount (kg)</label>
                  <input
                    id="hl-honey-kg"
                    name="honeyAmountKg"
                    type="number"
                    min={0}
                    step="0.1"
                    value={form.honeyAmountKg}
                    onChange={handleChange}
                  />
                  {validation.errors.honeyAmountKg && shouldShowError("honeyAmountKg") && (
                    <span className="apiary-form-error">{validation.errors.honeyAmountKg}</span>
                  )}
                </div>
                <div className="apiary-form-group">
                  <label htmlFor="hl-brood">Brood frames</label>
                  <input
                    id="hl-brood"
                    name="broodFrames"
                    type="number"
                    min={0}
                    value={form.broodFrames}
                    onChange={handleChange}
                  />
                  {validation.errors.broodFrames && shouldShowError("broodFrames") && (
                    <span className="apiary-form-error">{validation.errors.broodFrames}</span>
                  )}
                </div>
                <div className="apiary-form-group">
                  <label className="hivelog-checkbox-label">
                    <input
                      name="queenPresent"
                      type="checkbox"
                      checked={form.queenPresent}
                      onChange={handleChange}
                    />
                    Queen present
                  </label>
                </div>
                <div className="apiary-form-group">
                  <label htmlFor="hl-note">Note</label>
                  <textarea
                    id="hl-note"
                    name="note"
                    value={form.note}
                    onChange={handleChange}
                    maxLength={512}
                    placeholder="Free text..."
                  />
                  {validation.errors.note && shouldShowError("note") && (
                    <span className="apiary-form-error">{validation.errors.note}</span>
                  )}
                </div>
              </div>
              <div className="apiary-form-actions">
                <button
                  type="button"
                  className="apiary-form-cancel"
                  onClick={closeForm}
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="add-btn"
                  disabled={loading}
                >
                  {editingId ? "Save" : "Add"}
                </button>
              </div>
            </form>
          </div>
        )}
      </div>
    </div>
  );
};

export default HiveLogModal;
