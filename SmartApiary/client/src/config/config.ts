const API_BASE_URL = import.meta.env.VITE_API_URL || "https://localhost:7159";
const UPLOAD_BASE_URL = import.meta.env.VITE_UPLOAD_URL || "http://localhost:7253";
const APIARIES_PATH = "/api/apiaries";
const UPLOAD_PATH = "/api/upload-image";

export const CONFIG = {
  APIARIES_URL: `${API_BASE_URL}${APIARIES_PATH}`,
  UPLOAD_URL: `${UPLOAD_BASE_URL}${UPLOAD_PATH}`,
};
