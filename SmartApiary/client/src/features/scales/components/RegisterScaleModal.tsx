import React, { useState } from "react";

interface Props {
  hiveLabel: string;
  loading: boolean;
  onConfirm: (serialNumber: string, weightDropLimit?: number) => Promise<void>;
  onClose: () => void;
}

const RegisterScaleModal: React.FC<Props> = ({
  hiveLabel,
  loading,
  onConfirm,
  onClose,
}) => {
  const [serialNumber, setSerialNumber] = useState("");
  const [weightLimit, setWeightLimit] = useState("");
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

    let finalWeightLimit: number | undefined = undefined;
    if (weightLimit.trim() !== "") {
      const limitNum = parseFloat(weightLimit);

      if (isNaN(limitNum) || limitNum <= 0) {
        setError("Weight drop limit must be a positive number.");
        return;
      }
      finalWeightLimit = limitNum;
    }
    setError(null);
    try {
      await onConfirm(trimmed, finalWeightLimit); // Čekamo da backend odradi svoje
    } catch (serverError: any) {
      setError(serverError.message || "Failed to register scale.");
    }
  };

  return (
    <div className="apiary-modal-backdrop" onClick={onClose}>
      <form
        className="apiary-form register-scale-form"
        onSubmit={handleSubmit}
        onClick={(e) => e.stopPropagation()}
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

        {/* <p
          style={{
            color: "#94a3b8",
            fontSize: "13px",
            marginTop: "-5px",
            marginBottom: "15px",
          }}
        >
          Adding scale to hive:{" "}
          <strong style={{ color: "#f59e0b" }}>{hiveLabel}</strong>
        </p> */}
        <p className="register-scale-info">
          Adding scale to hive:{" "}
          <strong className="register-scale-highlight">{hiveLabel}</strong>
        </p>

        <div className="apiary-form-grid" scale-form-grid-block>
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
          {/* Prag za pad tezine */}
          <div className="apiary-form-group" scale-form-group-margin>
            <label
              htmlFor="scale-weight-limit"
              className="scale-label-optional-wrapper"
            >
              <span>Weight Drop Limit (kg)</span>
              <span className="scale-label-optional-text">(Optional)</span>
            </label>
            <input
              id="scale-weight-limit"
              type="number"
              step="0.1"
              placeholder="10.0 (default)"
              value={weightLimit}
              onChange={(e) => setWeightLimit(e.target.value)}
              disabled={loading}
            />
            <span className="scale-form-hint">
              Leave empty to use the system default of 10.0 kg.
            </span>
          </div>

          {error && (
            <span className="apiary-form-error scale-form-error-block">
              {error}
            </span>
          )}
        </div>

        <button type="submit" disabled={loading} className="scale-submit-btn">
          {loading ? "Registering..." : "Connect Scale"}
        </button>
      </form>
    </div>
  );
};

export default RegisterScaleModal;
