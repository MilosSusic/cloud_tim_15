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

const isLikelyImageUrl = (value?: string) => {
  if (!value) return false;
  return /^https?:\/\//i.test(value) || value.startsWith("/") || value.startsWith("data:image") || value.startsWith("blob:");
};

const parsePeriod = (period: string) => {
  const [rawStart, rawEnd] = period.split(" - ").map((part) => part.trim());
  const start = new Date(rawStart);
  const end = new Date(rawEnd);
  return {
    start,
    end,
    valid: !Number.isNaN(start.getTime()) && !Number.isNaN(end.getTime()),
  };
};

const formatDate = (value: Date) =>
  value.toLocaleDateString("sr-RS", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  });

const formatFloweringPeriod = (period: string) => {
  const { start, end, valid } = parsePeriod(period);
  if (!valid) return period;
  return `${formatDate(start)} - ${formatDate(end)}`;
};

const getCurrentCrop = (field: NearbyField) => {
  if (field.crops.length === 0) return null;
  const now = new Date();
  const active = field.crops.find((crop) => {
    const { start, end, valid } = parsePeriod(crop.floweringPeriod);
    if (!valid) return false;
    return now >= start && now <= end;
  });
  return active ?? field.crops[0];
};

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
                (() => {
                  const currentCrop = getCurrentCrop(field);
                  const currentCropHasImage = isLikelyImageUrl(currentCrop?.icon);

                  return (
                <Marker
                  key={field.fieldId}
                  position={[field.latitude, field.longitude]}
                  icon={greenIcon}
                  eventHandlers={{ click: () => onSelectField(field) }}
                >
                  <Popup>
                    <div className="field-popup">
                      <div className="field-popup-name">{field.fieldName}</div>
                      <div className="field-popup-owner">
                        <span>Owner:</span> {field.owner.firstName} {field.owner.lastName}<br />
                        <span>Email:</span> {field.owner.email}<br />
                        <span>Phone:</span> {field.owner.phone}
                      </div>

                      {currentCrop && (
                        <div className="field-popup-current-crop">
                          <div className="field-popup-current-title">Current crop</div>
                          {currentCropHasImage ? (
                            <img
                              className="field-popup-current-image"
                              src={currentCrop.icon}
                              alt={currentCrop.name}
                              onError={(event) => {
                                event.currentTarget.style.display = "none";
                              }}
                            />
                          ) : (
                            <div className="field-popup-current-fallback">{currentCrop.name}</div>
                          )}
                          <div className="field-popup-current-period">
                            {formatFloweringPeriod(currentCrop.floweringPeriod)}
                          </div>
                        </div>
                      )}
                    </div>
                  </Popup>
                </Marker>
                  );
                })()
              ) : null
            )}
        </MapContainer>
      </MapErrorBoundary>
    </div>
  </section>
);

export default ApiaryMap;
