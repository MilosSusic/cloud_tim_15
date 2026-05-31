import React, { useState } from "react";

interface Props {
  hiveLabel: string;
  loading: boolean;
  onConfirm: (serialNumber: string) => Promise<void>;
  onClose: () => void;
}

const RegisterScaleModal: React.FC<Props> = ({
  hiveLabel,
  loading,
  onConfirm,
  onClose,
}) => {
  const [serialNumber, setSerialNumber] = useState("");
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const trimmed = serialNumber.trim();

    if (!trimmed) {
      setError("Serial number is required.");
      return;
    }

    // Format: SA-YYYY-XXXXX
    const regex = /^SA-\d{4}-\d{5}$/;
    if (!regex.test(trimmed)) {
      setError("Format must be SA-YYYY-XXXXX (e.g., SA-2026-00001).");
      return;
    }

    setError(null);
    try {
      await onConfirm(trimmed); // 👈 Čekamo da backend odradi svoje
    } catch (serverError: any) {
      setError(serverError.message || "Failed to register scale.");
    }
  };

  return (
    <div className="apiary-modal-backdrop" onClick={onClose}>
      <form
        className="apiary-form"
        onSubmit={handleSubmit}
        onClick={(e) => e.stopPropagation()}
        style={{ maxWidth: "400px" }}
      >
        <div className="apiary-form-header">
          <h3 className="apiary-form-title">Register Scale</h3>
          <button
            type="button"
            className="apiary-info-close"
            onClick={onClose}
            aria-label="Close"
          >
            ×
          </button>
        </div>

        <p
          style={{
            color: "#94a3b8",
            fontSize: "13px",
            marginTop: "-5px",
            marginBottom: "15px",
          }}
        >
          Adding scale to hive:{" "}
          <strong style={{ color: "#f59e0b" }}>{hiveLabel}</strong>
        </p>

        <div className="apiary-form-grid" style={{ display: "block" }}>
          <div className="apiary-form-group">
            <label htmlFor="scale-sn">Scale Serial Number</label>
            <input
              id="scale-sn"
              placeholder="SA-2026-00001"
              value={serialNumber}
              onChange={(e) => setSerialNumber(e.target.value)}
              disabled={loading}
            />
            {error && <span className="apiary-form-error">{error}</span>}
          </div>
        </div>

        <button type="submit" disabled={loading} style={{ marginTop: "15px" }}>
          {loading ? "Registering..." : "Connect Scale"}
        </button>
      </form>
    </div>
  );
};

export default RegisterScaleModal;
