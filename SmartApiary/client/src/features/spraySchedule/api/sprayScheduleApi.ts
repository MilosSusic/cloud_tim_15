import { getToken } from '../../auth/api/authApi';
import { CONFIG } from '../../../config/config';
import type {SpraySchedule} from '../types/SpraySchedule'

const authHeaders = () => ({
  Authorization: `Bearer ${getToken()}`,
});

export async function createSpray(form: any) {
    const token = getToken();

    const sprayData = {
        fieldId : form.fieldId,
        startDateTime : form.startDateTime,
        durationHours : form.durationHours,
        pesticideType : form.pesticideType,
        status : form.status,
        createdAt : form.createdAt
    };
    console.log('Payload:', sprayData);
    const resp = await fetch(CONFIG.SPRAY_URL, {
        method: 'POST',
        headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(sprayData)
    });
}
export async function updateSpray(id: string, form: any) {
  const token = getToken();

  const payload = {
    id,
    startDateTime : form.startDateTime,
    durationHours : form.durationHours,
    pesticideType : form.pesticideType,
    status : form.status
  };
  console.log(payload.dstatus);
  const resp = await fetch(`${CONFIG.SPRAY_URL}/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
    body: JSON.stringify(payload),
  });
  const text = await resp.text();
  console.log(text);
  if (!resp.ok) throw new Error('Error with update of spraySchedule');
  
  
  return text ? JSON.parse(text) : null;
}

export async function getSprayByField(fieldId: string): Promise<SpraySchedule[]> {
  const resp = await fetch(`${CONFIG.SPRAY_URL}/by-field/${fieldId}`, {
    headers: authHeaders(),
  });
  if (!resp.ok) {
    const errorText = await resp.text();
    console.error("Backend error:", errorText);
    throw new Error("Error with fetching spray from field.");
  }
  const data: SpraySchedule[] = await resp.json();
  return data;
}
