import axios from 'axios';
import { getToken } from '../../auth/api/authApi';
import { CONFIG } from '../../../config/config';

const authHeaders = () => ({
  Authorization: `Bearer ${getToken()}`,
});

export async function getNearbyFieldsWithCrops() {
  try {
    const response = await axios.get(CONFIG.NEARBY_FIELDS_WITH_CROPS_URL, {
      headers: authHeaders(),
    });
    return response.data;
  } catch {
    throw new Error('Failed to fetch nearby fields around apiaries');
  }
}
