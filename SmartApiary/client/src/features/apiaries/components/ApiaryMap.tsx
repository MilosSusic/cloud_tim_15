import React from "react";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import L from "leaflet";
import type { Apiary } from "../types/Apiary";
import type { NearbyField } from "../types/NearbyField";
import MapErrorBoundary from "./MapErrorBoundary";
import "leaflet/dist/leaflet.css";

const orangeIcon = new L.Icon({
  iconUrl: "https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-orange.png",
  shadowUrl: "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png",
  iconSize: [25, 41], iconAnchor: [12, 41], popupAnchor: [1, -34], shadowSize: [41, 41],
});
const greenIcon = new L.Icon({
  iconUrl: "https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-green.png",
  shadowUrl: "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png",
  iconSize: [25, 41], iconAnchor: [12, 41], popupAnchor: [1, -34], shadowSize: [41, 41],
});

interface Props {
  apiaries: Apiary[];
  nearbyFields: NearbyField[];
  fieldsLoading: boolean;
  onSelectApiary: (apiary: Apiary) => void;
  onSelectField: (field: NearbyField) => void;
}

const ApiaryMap: React.FC<Props> = ({ apiaries, nearbyFields, fieldsLoading, onSelectApiary, onSelectField }) => (
  <section className="apiary-map">
    <div className="map-section">
      <MapErrorBoundary>
        <MapContainer center={[45.25, 19.85]} zoom={6} style={{ height: "100%", width: "100%" }}>
          <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
          {apiaries.map((apiary) =>
            typeof apiary.latitude === "number" && typeof apiary.longitude === "number" ? (
              <Marker
                key={apiary.id}
                position={[apiary.latitude, apiary.longitude]}
                icon={orangeIcon}
                eventHandlers={{ click: () => onSelectApiary(apiary) }}
              >
                <Popup>
                  <div className="apiary-popup">
                    <img src={apiary.thumbnailUrl || apiary.imageUrl} alt={apiary.name} />
                    <div className="apiary-popup-name">{apiary.name}</div>
                    <p className="apiary-popup-desc">{apiary.description}</p>
                    <div className="apiary-popup-coords">Lat: {apiary.latitude}, Lng: {apiary.longitude}</div>
                  </div>
                </Popup>
              </Marker>
            ) : null
          )}
          {!fieldsLoading &&
            nearbyFields.map((field) =>
              typeof field.latitude === "number" && typeof field.longitude === "number" ? (
                <Marker
                  key={field.fieldId}
                  position={[field.latitude, field.longitude]}
                  icon={greenIcon}
                  eventHandlers={{ click: () => onSelectField(field) }}
                >
                  <Popup>
                    <div className="field-popup" style={{ minWidth: 220, padding: 8 }}>
                      <div style={{ fontWeight: "bold", fontSize: 16, marginBottom: 4 }}>{field.fieldName}</div>
                      <div style={{ marginBottom: 4 }}>
                        <span style={{ fontWeight: "bold" }}>Owner:</span> {field.owner.firstName} {field.owner.lastName}<br />
                        <span style={{ fontWeight: "bold" }}>Email:</span> {field.owner.email}<br />
                        <span style={{ fontWeight: "bold" }}>Phone:</span> {field.owner.phone}
                      </div>
                      <div style={{ marginTop: 4 }}>
                        <span style={{ fontWeight: "bold" }}>Crops:</span>
                        {field.crops.length === 0 ? (
                          <span style={{ marginLeft: 6, color: "#888" }}>No crops</span>
                        ) : (
                          <ul style={{ margin: 0, paddingLeft: 18 }}>
                            {field.crops.map((crop, idx) => (
                              <li key={idx} style={{ display: "flex", alignItems: "center", marginBottom: 2 }}>
                                {crop.icon && <img src={crop.icon} alt={crop.name} style={{ width: 16, height: 16, marginRight: 4 }} />}
                                <span style={{ color: "#228B22", fontWeight: 500 }}>{crop.name}</span>
                                <span style={{ marginLeft: 4, fontSize: 12, color: "#666" }}>({crop.floweringPeriod})</span>
                              </li>
                            ))}
                          </ul>
                        )}
                      </div>
                    </div>
                  </Popup>
                </Marker>
              ) : null
            )}
        </MapContainer>
      </MapErrorBoundary>
    </div>
  </section>
);

export default ApiaryMap;
