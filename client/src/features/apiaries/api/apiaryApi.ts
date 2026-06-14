import axios from 'axios';
import { CONFIG } from '../../../shared/config/config';
import { extractAxiosErrorMessage } from '../../../shared/api/extractAxiosErrorMessage';

const authJsonHeaders = () => ({
  'Content-Type': 'application/json',
});

const extractUploadUrl = (uploadResponse: unknown): string | undefined => {
  if (typeof uploadResponse !== 'object' || uploadResponse === null) return undefined;
  const responseWithUrls = uploadResponse as { urls?: unknown };
  return Array.isArray(responseWithUrls.urls)
    ? (responseWithUrls.urls[0] as string | undefined)
    : undefined;
};

export async function getApiaries() {
  try {
    const response = await axios.get(CONFIG.APIARIES_URL);
    return response.data;
  } catch (error) {
    throw new Error(extractAxiosErrorMessage(error, 'Failed to fetch apiaries'));
  }
}

export async function createApiary(form: any) {
  try {
    const formData = new FormData();
    formData.append('file', form.file);

    const uploadResponse = await axios.post(CONFIG.UPLOAD_URL, formData);
    const url = extractUploadUrl(uploadResponse.data);

    const apiaryData = {
      name: form.name,
      description: form.description,
      latitude: parseFloat(form.latitude),
      longitude: parseFloat(form.longitude),
      imageUrl: url,
      thumbnailUrl: url,
    };

    const response = await axios.post(CONFIG.APIARIES_URL, apiaryData, {
      headers: authJsonHeaders(),
    });

    return response.data;
  } catch (error) {
    throw new Error(extractAxiosErrorMessage(error, 'Failed to create apiary'));
  }
}

export async function updateApiary(id: string, form: any) {
  try {
    let imageUrl: string | null = null;
    let thumbnailUrl: string | null = null;

    if (form.file) {
      const formData = new FormData();
      formData.append('file', form.file);
      const uploadResponse = await axios.post(CONFIG.UPLOAD_URL, formData);
      const url = extractUploadUrl(uploadResponse.data);
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

    const response = await axios.put(`${CONFIG.APIARIES_URL}/${id}`, payload, {
      headers: authJsonHeaders(),
    });

    return response.data ?? null;
  } catch (error) {
    throw new Error(extractAxiosErrorMessage(error, 'Failed to update apiary'));
  }
}
