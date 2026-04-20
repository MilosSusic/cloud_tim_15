
import { getToken } from '../../auth/api/authApi';
import { CONFIG } from '../../../config/config';

export async function getApiaries() {
  const token = getToken();
  const resp = await fetch(CONFIG.APIARIES_URL, {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  if (!resp.ok) throw new Error('Greška pri dohvatanju pčelinjaka');
  return resp.json();
}

export async function createApiary(form: any) {
  const token = getToken();
  // 1. Upload image
  const formData = new FormData();
  formData.append('file', form.file);
  const uploadResp = await fetch(CONFIG.UPLOAD_URL, { method: 'POST', body: formData });
  if (!uploadResp.ok) throw new Error('Greška pri uploadu slike');
  const uploadJson = await uploadResp.clone().json();
  console.log('Upload response:', uploadJson);
  const url = Array.isArray(uploadJson.urls) ? uploadJson.urls[0] : undefined;
  console.log('Dobijen url:', url);

  // 2. Create apiary
  const apiaryData = {
    name: form.name,
    description: form.description,
    latitude: parseFloat(form.latitude),
    longitude: parseFloat(form.longitude),
    imageUrl: url,
    thumbnailUrl: url,
    // ownerId: ... // Dodaj ownerId ako je potrebno
  };
  console.log('Payload za backend:', apiaryData);
  const resp = await fetch(CONFIG.APIARIES_URL, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(apiaryData)
  });
  if (!resp.ok) throw new Error('Greška pri kreiranju pčelinjaka');
  return resp.json();
}
