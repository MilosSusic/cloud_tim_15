import { getToken } from '../../auth/api/authApi';
import { CONFIG } from '../../../config/config';
import type {Crop} from '../types/Crop'

const authHeaders = () => ({
  Authorization: `Bearer ${getToken()}`,
});

export async function createCrop(form: any) {
    const token = getToken();

    const cropData = {
        fieldId: form.fieldId,
        description: form.description,
        cropName: form.cropName,
        FloweringPeriodStart: form.FloweringPeriodStart,
        FloweringPeriodEnd: form.FloweringPeriodEnd
    };
    console.log('Payload:', cropData);
    const resp = await fetch(CONFIG.CROP_URL, {
        method: 'POST',
        headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(cropData)
    });
    if (!resp.ok) {
        const errorText = await resp.text();
        console.error("Backend error:", errorText);
        throw new Error(errorText || "Error with creating crop!");
    }
    return resp.json();
}
export async function getCropsByField(fieldId: string): Promise<Crop[]> {
  const resp = await fetch(`${CONFIG.CROP_URL}/by-field/${fieldId}`, {
    headers: authHeaders(),
  });
  if (!resp.ok) {
    const errorText = await resp.text();
    console.error("Backend error:", errorText);
    throw new Error("Error with fetching crops from field.");
  }
  const data: Crop[] = await resp.json();
  return data;
}
export async function harvestCrop(cropId: string) {
  const resp = await fetch(`${CONFIG.CROP_URL}/${cropId}`, {
    method: 'DELETE',
    headers: authHeaders(),
  });
  if (!resp.ok) {
    const errorText = await resp.text();
    console.error("Backend error:", errorText);
    throw new Error("Error with harvesting crop.");
  }
}