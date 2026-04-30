import { getToken } from '../../auth/api/authApi';
import { CONFIG } from '../../../config/config';

export async function getNearbyFieldsWithCrops() {
  const token = getToken();
  const resp = await fetch(CONFIG.NEARBY_FIELDS_WITH_CROPS_URL, {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  if (!resp.ok) throw new Error('Greška pri dohvatanju parcela u blizini pčelinjaka');
  return resp.json();
}
