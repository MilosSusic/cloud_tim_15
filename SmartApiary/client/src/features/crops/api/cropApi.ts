import { CONFIG } from '../../../shared/config/config';
import type {Crop} from '../types/Crop'
import axios from 'axios';

const extractAxiosErrorMessage = (error: unknown, fallback: string): string => {
  if (axios.isAxiosError(error)) {
    const payload = error.response?.data;

    if (typeof payload === 'string' && payload.trim()) {
      return payload;
    }

    if (payload && typeof payload === 'object') {
      const data = payload as { message?: unknown; title?: unknown; errors?: Record<string, string[] | string> };

      if (typeof data.message === 'string' && data.message.trim()) {
        return data.message;
      }

      if (typeof data.title === 'string' && data.title.trim()) {
        return data.title;
      }

      if (data.errors && typeof data.errors === 'object') {
        const first = Object.values(data.errors)[0];
        if (Array.isArray(first) && first.length > 0) {
          return String(first[0]);
        }
        if (typeof first === 'string' && first.trim()) {
          return first;
        }
      }
    }
  }

  return fallback;
};

export async function createCrop(form: any) {
  const cropData = {
    fieldId: form.fieldId,
    description: form.description,
    cropName: form.cropName,
    FloweringPeriodStart: form.FloweringPeriodStart,
    FloweringPeriodEnd: form.FloweringPeriodEnd,
  };

  console.log("Payload:", cropData);

  try {
    const resp = await axios.post(CONFIG.CROP_URL, cropData, {
      headers: {
        "Content-Type": "application/json",
      },
    });

    return resp.data; 
  } catch (error) {
    throw new Error(extractAxiosErrorMessage(error, 'Error with creating crop!'));
  }
}

export async function getCropsByField(fieldId: string): Promise<Crop[]> {
  try {
    const resp = await axios.get(`${CONFIG.CROP_URL}/by-field/${fieldId}`);
    return resp.data as Crop[];
  } catch (error: any) {
    if (error.response) {
      console.error("Backend error:", error.response.data);
      throw new Error("Error with fetching crops from field.");
    }
    throw new Error("Network error while fetching crops.");
  }
}

export async function harvestCrop(cropId: string) {
  try {
    const resp = await axios.delete(`${CONFIG.CROP_URL}/${cropId}`);
    return resp.data;
  } catch (error: any) {
    if (error.response) {
      console.error("Backend error:", error.response.data);
      throw new Error("Error with harvesting crop.");
    }
    throw new Error("Network error while harvesting crop.");
  }
}
