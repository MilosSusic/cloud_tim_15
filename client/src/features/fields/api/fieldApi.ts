import { CONFIG } from '../../../shared/config/config';
import axios from 'axios';

export async function createField(form: any) {
  const fieldData = {
    fieldName: form.fieldName,
    description: form.description,
    latitude: parseFloat(form.latitude),
    longitude: parseFloat(form.longitude),
    imageUrl: "",
    thumbnailUrl: "",
  };

  console.log("Payload:", fieldData);

  try {
    const resp = await axios.post(CONFIG.FIELD_URL, fieldData, {
      headers: {
        "Content-Type": "application/json",
      },
    });
    return resp.data;
  } catch (error: any) {
    if (error.response) {
      console.error("Backend error:", error.response.data);
      throw new Error(error.response.data || "Error with creating field!");
    }
    throw new Error("Network error while creating field!");
  }
}

export async function getFields() {
  try {
    const resp = await axios.get(CONFIG.FIELD_URL);
    return resp.data;
  } catch (error: any) {
    if (error.response) {
      console.error("Backend error:", error.response.data);
      throw new Error("Error with fetching fields.");
    }
    throw new Error("Network error while fetching fields.");
  }
}

export async function updateField(form: any) {
  let imageUrl: string | null = null;
  let thumbnailUrl: string | null = null;

  if (form.file) {
    const formData = new FormData();
    formData.append("file", form.file);

    try {
      const uploadResp = await axios.post(CONFIG.UPLOAD_FIELD_IMAGE_URL, formData);

      const uploadJson = uploadResp.data;
      const url = Array.isArray(uploadJson.urls) ? uploadJson.urls[0] : undefined;
      imageUrl = url ?? null;
      thumbnailUrl = url ?? null;
    } catch (error: any) {
      throw new Error("Failed to upload field image");
    }
  }

  const fieldData = {
    id: form.id,
    fieldName: form.fieldName,
    description: form.description,
    latitude: form.latitude,
    longitude: form.longitude,
    imageUrl: imageUrl || form.imageUrl || "",
    thumbnailUrl: thumbnailUrl || form.thumbnailUrl || "",
  };

  console.log("Payload:", fieldData);

  try {
    await axios.put(`${CONFIG.FIELD_URL}/${form.id}`, fieldData, {
      headers: {
        "Content-Type": "application/json",
      },
    });
  } catch (error: any) {
    if (error.response) {
      console.error("Backend error:", error.response.data);
      throw new Error(error.response.data || "Error with updating field!");
    }
    throw new Error("Network error while updating field!");
  }
}
