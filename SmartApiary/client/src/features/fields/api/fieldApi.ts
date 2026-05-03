import { getToken } from '../../auth/api/authApi';
import { CONFIG } from '../../../config/config';

export async function createField(form: any) {
    const token = getToken();

    const fieldData = {
        fieldName: form.fieldName,
        description: form.description,
        latitude: parseFloat(form.latitude),
        longitude: parseFloat(form.longitude),
        imageUrl: "",
        thumbnailUrl: "",
    };
    console.log('Payload:', fieldData);
    const resp = await fetch(CONFIG.FIELD_URL, {
        method: 'POST',
        headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(fieldData)
    });
    if (!resp.ok) {
        const errorText = await resp.text();
        console.error("Backend error:", errorText);
        throw new Error(errorText || "Error with creating field!");
    }
    return resp.json();
}
export async function getFields() {
  const token = getToken();
  console.log("Fetching fields from:", CONFIG.FIELD_URL);
  const resp = await fetch(CONFIG.FIELD_URL, {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  
  
  if (!resp.ok){
    const errorText = await resp.text();
    console.error("Error fetching fields:", errorText);
  }
  return resp.json();
}

export async function updateField(form: any) {
    const token = getToken();

    let imageUrl: string | null = null;
    let thumbnailUrl: string | null = null;
      if (form.file) {
        const formData = new FormData();
        formData.append('file', form.file);
        const uploadResp = await fetch(CONFIG.UPLOAD_FIELD_IMAGE_URL, { method: 'POST', body: formData });
        if (!uploadResp.ok) throw new Error('Failed to upload field image');
        const uploadJson = await uploadResp.json();
        const url = Array.isArray(uploadJson.urls) ? uploadJson.urls[0] : undefined;
        imageUrl = url ?? null;
        thumbnailUrl = url ?? null;
      }

    const fieldData = {
        id: form.id,
        fieldName: form.fieldName,
        description: form.description,
        latitude: form.latitude,
        longitude: form.longitude,
        imageUrl: imageUrl,
        thumbnailUrl: thumbnailUrl,
    };
    console.log('Payload:', fieldData);
    const resp = await fetch(`${CONFIG.FIELD_URL}/${form.id}`, {
        method: 'PUT',
        headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(fieldData)
    });
    if (!resp.ok) {
        const errorText = await resp.text();
        console.error("Backend error:", errorText);
        throw new Error(errorText || "Error with updating field!");
    }
}