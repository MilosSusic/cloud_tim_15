import React, { useEffect, useState } from "react";
import { getFields } from "../../fields/api/fieldApi";
import { getSprayLogs } from "../api/sprayScheduleApi";
import type { Field } from "../../fields/types/Field";
import "../layout/SprayLog.css";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../../shared/auth/AuthContext";

interface SprayLog {
  id: string;
  fieldName: string;
  cropName: string | null;
  startDateTime: string;
  endDateTime: string;
  pesticideType: string | null;
  windSpeed: number | null;
  temperature: number | null;
  weatherDescription: string | null;
  createdAt: string;
}

const SprayLogPage: React.FC = () => {
  const [fields, setFields] = useState<Field[]>([]);
  const [logs, setLogs] = useState<SprayLog[]>([]);
  const [selectedFieldId, setSelectedFieldId] = useState<string>("");
  const [from, setFrom] = useState<string>("");
  const [to, setTo] = useState<string>("");
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const navigate = useNavigate();
  const { logout } = useAuth();

  useEffect(() => {
    getFields().then(setFields).catch(console.error);
  }, []);

  const fetchLogs = async () => {
    setLoading(true);
    try {
      const data = await getSprayLogs(
        selectedFieldId || undefined,
        from || undefined,
        to || undefined
      );
      setLogs(data);
      setMessage(data.length === 0 ? "No logs found." : null);
    } catch (err) {
      console.error(err);
      setMessage("Failed to fetch spray logs.");
    }
    setLoading(false);
  };

  const handleExportPdf = async () => {
    const { default: jsPDF } = await import("jspdf");
    const { default: autoTable } = await import("jspdf-autotable");

    const doc = new jsPDF();
    doc.setFontSize(16);
    doc.text("Spray Treatment Log", 14, 15);
    doc.setFontSize(10);
    doc.text(`Generated: ${new Date().toLocaleString()}`, 14, 22);

    autoTable(doc, {
      startY: 28,
      head: [[
              "Field Name",
              "Crop Type", 
              "Start Date & Time",
              "End Date & Time",
              "Pesticide Type",
              "Wind Speed (m/s)",
              "Temperature (°C)",
              "Weather Conditions"
            ]],
      body: logs.map(log => [
        log.fieldName,
        log.cropName ?? "N/A",
        new Date(log.startDateTime).toLocaleString(),
        new Date(log.endDateTime).toLocaleString(),
        log.pesticideType ?? "N/A",
        log.windSpeed?.toFixed(1) ?? "N/A",
        log.temperature?.toFixed(1) ?? "N/A",
        log.weatherDescription ?? "N/A",
      ]),
      styles: { fontSize: 8 },
      headStyles: { fillColor: [34, 197, 94] },
    });

    doc.save("spray-log.pdf");
  };

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="spray-log-page">
      <div className="spray-log-header">
        <div>
          <div className="spray-log-eyebrow">Smart Agriculture</div>
          <h2>Spray Treatment Log</h2>
        </div>
        <button className="spray-log-logout-btn" onClick={handleLogout}>
          ⎋ Logout
        </button>
      </div>

      <div className="spray-log-filters">
        <div className="filter-group">
          <label>Field Name</label>
          <select
            value={selectedFieldId}
            onChange={(e) => setSelectedFieldId(e.target.value)}
          >
            <option value="">All Fields</option>
            {fields.map((f) => (
              <option key={f.id} value={f.id}>{f.fieldName}</option>
            ))}
          </select>
        </div>

        <div className="filter-group">
          <label>Start Date <span className="date-format">(MM/DD/YYYY)</span></label>
          <input
            type="date"
            value={from}
            onChange={(e) => setFrom(e.target.value)}
          />
        </div>

        <div className="filter-group">
          <label>End Date <span className="date-format">(MM/DD/YYYY)</span></label>
          <input
            type="date"
            value={to}
            onChange={(e) => setTo(e.target.value)}
          />
        </div>

        <button className="spray-log-btn" onClick={fetchLogs}>
          🔍 Search
        </button>

        {logs.length > 0 && (
          <button className="spray-log-btn secondary" onClick={handleExportPdf}>
            📄 Export PDF
          </button>
        )}
      </div>
      {message && <div className="spray-log-message">{message}</div>}

      {loading && <div className="spray-log-message">Loading...</div>}

      {logs.length > 0 && (
        <div className="spray-log-table-wrapper">
          <table className="spray-log-table">
            <thead>
              <tr>
                <th>Field Name</th>
                <th>Crop Type</th>
                <th>Start Date & Time</th>
                <th>End Date & Time</th>
                <th>Pesticide Type</th>
                <th>Wind Speed (m/s)</th>
                <th>Temperature (°C)</th>
                <th>Weather Conditions</th>
              </tr>
            </thead>
            <tbody>
              {logs.map((log) => (
                <tr key={log.id}>
                  <td>{log.fieldName}</td>
                  <td>{log.cropName ?? "N/A"}</td>
                  <td>{new Date(log.startDateTime).toLocaleString()}</td>
                  <td>{new Date(log.endDateTime).toLocaleString()}</td>
                  <td>{log.pesticideType ?? "N/A"}</td>
                  <td>{log.windSpeed?.toFixed(1) ?? "N/A"}</td>
                  <td>{log.temperature?.toFixed(1) ?? "N/A"}</td>
                  <td>{log.weatherDescription ?? "N/A"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default SprayLogPage;