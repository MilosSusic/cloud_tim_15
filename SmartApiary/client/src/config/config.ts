const API_BASE_URL = import.meta.env.VITE_API_URL || "https://localhost:7159";
const UPLOAD_BASE_URL = import.meta.env.VITE_UPLOAD_URL || "http://localhost:7253";
const APIARIES_PATH = "/api/apiaries";
const HIVES_PATH = "/api/hives";
const HIVE_LOG_ENTRIES_PATH = "/api/HiveLogEntries";
const FIELD_PATH="/api/Fields";
const CROP_PATH="/api/Crop";
const SPRAY_PATH="/api/SpraySchedule";
const UPLOAD_PATH = "/api/upload-image";

export const CONFIG = {
  APIARIES_URL: `${API_BASE_URL}${APIARIES_PATH}`,
  HIVES_URL: `${API_BASE_URL}${HIVES_PATH}`,
  HIVE_LOG_ENTRIES_URL: `${API_BASE_URL}${HIVE_LOG_ENTRIES_PATH}`,
  UPLOAD_URL: `${UPLOAD_BASE_URL}${UPLOAD_PATH}`,
  FIELD_URL: `${API_BASE_URL}${FIELD_PATH}`,
  CROP_URL: `${API_BASE_URL}${CROP_PATH}`,
  SPRAY_URL: `${API_BASE_URL}${SPRAY_PATH}`,
};
