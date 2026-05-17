import axios from 'axios';
import { CONFIG } from '../../../shared/config/config';

export async function getNearbyFieldsWithCrops() {
  try {
    const response = await axios.get(CONFIG.NEARBY_FIELDS_WITH_CROPS_URL);
    return response.data;
  } catch {
    throw new Error('Failed to fetch nearby fields around apiaries');
  }
}
