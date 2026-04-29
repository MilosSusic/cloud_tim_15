import { getToken } from '../../auth/api/authApi';
import { CONFIG } from '../../../config/config';

export async function createField(form: any) {
    const token = getToken();
    // 1. Upload image
    const formData = new FormData();
    formData.append('file', form.file);
    const uploadResp = await fetch(CONFIG.UPLOAD_URL, { method: 'POST', body: formData });
    if (!uploadResp.ok) throw new Error('Error in uploading photo');
    const uploadJson = await uploadResp.clone().json();
    console.log('Upload response:', uploadJson);
    const url = Array.isArray(uploadJson.urls) ? uploadJson.urls[0] : undefined;
    console.log('Photo url:', url);

    const fieldData = {
        fieldName: form.fieldName,
        description: form.description,
        latitude: parseFloat(form.latitude),
        longitude: parseFloat(form.longitude),
        imageUrl: url,
        thumbnailUrl: url,
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
    console.error("Error fetching fields:", errorText);
  }
  return resp.json();
}