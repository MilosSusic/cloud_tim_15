import { CONFIG } from '../../../shared/config/config';
import type {Crop} from '../types/Crop'
import axios from 'axios';
import { extractAxiosErrorMessage } from '../../../shared/api/extractAxiosErrorMessage';

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
