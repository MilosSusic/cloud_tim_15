import { getToken } from '../../auth/api/authApi';
import { CONFIG } from '../../../config/config';
import type {SpraySchedule} from '../types/SpraySchedule'
import type { SprayStatus } from '../types/SprayStatus';

const authHeaders = () => ({
  Authorization: `Bearer ${getToken()}`,
});

export async function createSpray(form: any): Promise<{ sprayScheduleId: string, notifiedBeekeepersCount: number, weatherWarning: string | null }> {
    const token = getToken();

    const sprayData = {
        fieldId: form.fieldId,
        startDateTime: form.startDateTime,
        durationHours: form.durationHours,
        pesticideType: form.pesticideType,
        status: form.status,
        createdAt: form.createdAt
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
    console.log('Response status:', resp.status);

    if (!resp.ok) throw new Error('Error creating spray schedule.');
    return await resp.json();
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
  console.log(payload.status);
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

export async function rescheduleSpray(id: string, startDateTime: string){
  const token = getToken();

  const payload = {
    id,
    startDateTime
  };

  const resp = await fetch(`${CONFIG.SPRAY_URL}/reschedule/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
    body: JSON.stringify(payload),
  });

  if (!resp.ok) {
    const errorText = await resp.text();
    console.error("Backend error:", errorText);
    throw new Error("Error with rescheduling spray.");
  }
}

export async function changeSprayStatus(id: string, status: SprayStatus) {
  const token = getToken();

  const payload = {
    id,
    newStatus: status
  };

  const resp = await fetch(`${CONFIG.SPRAY_URL}/status/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
    body: JSON.stringify(payload),
  });

  if (!resp.ok) {
    const errorText = await resp.text();
    console.error("Backend error:", errorText);
    throw new Error("Error with changing spray status.");
  }

}

export async function getSprayLogs(
  fieldId?: string,
  from?: string,
  to?: string
): Promise<any[]> {
  const params = new URLSearchParams();
  if (fieldId) params.append("fieldId", fieldId);
  if (from) params.append("from", from);
  if (to) params.append("to", to);

  const resp = await fetch(`${CONFIG.SPRAY_URL}/logs?${params.toString()}`, {
    headers: authHeaders(),
  });
  if (!resp.ok) throw new Error("Error fetching spray logs.");
  return await resp.json();
}
