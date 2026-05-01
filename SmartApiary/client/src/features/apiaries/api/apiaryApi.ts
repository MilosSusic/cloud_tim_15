
import { getToken } from '../../auth/api/authApi';
import { CONFIG } from '../../../config/config';

export async function getApiaries() {
  const token = getToken();
  const resp = await fetch(CONFIG.APIARIES_URL, {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  if (!resp.ok) throw new Error('Failed to fetch apiaries');
  return resp.json();
}

export async function createApiary(form: any) {
  const token = getToken();
  // 1. Upload image
  const formData = new FormData();
  formData.append('file', form.file);
  const uploadResp = await fetch(CONFIG.UPLOAD_URL, { method: 'POST', body: formData });
  if (!uploadResp.ok) throw new Error('Failed to upload image');
  const uploadJson = await uploadResp.clone().json();
  console.log('Upload response:', uploadJson);
  const url = Array.isArray(uploadJson.urls) ? uploadJson.urls[0] : undefined;
  console.log('Uploaded image URL:', url);

  // 2. Create apiary
  const apiaryData = {
    name: form.name,
    description: form.description,
    latitude: parseFloat(form.latitude),
    longitude: parseFloat(form.longitude),
    imageUrl: url,
    thumbnailUrl: url,
    // ownerId: ... // Add ownerId if needed
  };
  console.log('Backend payload:', apiaryData);
  const resp = await fetch(CONFIG.APIARIES_URL, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(apiaryData)
  });
  if (!resp.ok) throw new Error('Failed to create apiary');
  return resp.json();
}

export async function updateApiary(id: string, form: any) {
  const token = getToken();

  // 1. Optional new image upload
  let imageUrl: string | null = null;
  let thumbnailUrl: string | null = null;
  if (form.file) {
    const formData = new FormData();
    formData.append('file', form.file);
    const uploadResp = await fetch(CONFIG.UPLOAD_URL, { method: 'POST', body: formData });
    if (!uploadResp.ok) throw new Error('Failed to upload image');
    const uploadJson = await uploadResp.json();
    const url = Array.isArray(uploadJson.urls) ? uploadJson.urls[0] : undefined;
    imageUrl = url ?? null;
    thumbnailUrl = url ?? null;
  }

  const payload = {
    id,
    name: form.name,
    description: form.description,
    latitude: parseFloat(form.latitude),
    longitude: parseFloat(form.longitude),
    imageUrl,
    thumbnailUrl,
  };

  const resp = await fetch(`${CONFIG.APIARIES_URL}/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
    body: JSON.stringify(payload),
  });
  if (!resp.ok) throw new Error('Failed to update apiary');
  // Some PUT endpoints return 204 No Content
  const text = await resp.text();
  return text ? JSON.parse(text) : null;
}
