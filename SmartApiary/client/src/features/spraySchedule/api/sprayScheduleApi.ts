import { getToken } from '../../auth/api/authApi';
import { CONFIG } from '../../../config/config';
import type {SpraySchedule} from '../types/SpraySchedule'
import type { SprayStatus } from '../types/SprayStatus';
import axios from 'axios';

const authHeaders = () => ({
  Authorization: `Bearer ${getToken()}`,
});

export async function createSpray(form: any): Promise<{ sprayScheduleId: string, notifiedBeekeepersCount: number, weatherWarning: string | null }> {
  const sprayData = {
    fieldId: form.fieldId,
    startDateTime: form.startDateTime,
    durationHours: form.durationHours,
    pesticideType: form.pesticideType,
    status: form.status,
    createdAt: form.createdAt,
  };

  console.log("Payload:", sprayData);

  try {
    const resp = await axios.post(CONFIG.SPRAY_URL, sprayData, {
      headers: {
        "Content-Type": "application/json",
        ...authHeaders(),
      },
    });
    console.log("Response status:", resp.status);
    return resp.data;
  } catch (error: any) {
    console.error("Error creating spray schedule:", error.response?.data || error.message);
    throw new Error("Error creating spray schedule.");
  }
}

export async function updateSpray(id: string, form: any) {
  const payload = {
    id,
    startDateTime: form.startDateTime,
    durationHours: form.durationHours,
    pesticideType: form.pesticideType,
    status: form.status,
  };

  console.log(payload.status);

  try {
    const resp = await axios.put(`${CONFIG.SPRAY_URL}/${id}`, payload, {
      headers: {
        "Content-Type": "application/json",
        ...authHeaders(),
      },
    });

    console.log(resp.data);
    return resp.data || null;
  } catch (error: any) {
    console.error("Error with update of spraySchedule:", error.response?.data || error.message);
    throw new Error("Error with update of spraySchedule");
  }
}

export async function getSprayByField(fieldId: string): Promise<SpraySchedule[]> {
  try {
    const resp = await axios.get(`${CONFIG.SPRAY_URL}/by-field/${fieldId}`, {
      headers: {
        ...authHeaders(),
      },
    });
    return resp.data as SpraySchedule[];
  } catch (error: any) {
    console.error("Error with fetching spray from field:", error.response?.data || error.message);
    throw new Error("Error with fetching spray from field.");
  }
}

export async function rescheduleSpray(id: string, startDateTime: string) {
  const payload = { id, startDateTime };

  try {
    await axios.put(`${CONFIG.SPRAY_URL}/reschedule/${id}`, payload, {
      headers: {
        "Content-Type": "application/json",
        ...authHeaders(),
      },
    });
  } catch (error: any) {
    console.error("Error with rescheduling spray:", error.response?.data || error.message);
    throw new Error("Error with rescheduling spray.");
  }
}

export async function changeSprayStatus(id: string, status: SprayStatus) {
  const payload = { id, newStatus: status };

  try {
    await axios.put(`${CONFIG.SPRAY_URL}/status/${id}`, payload, {
      headers: {
        "Content-Type": "application/json",
        ...authHeaders(),
      },
    });
  } catch (error: any) {
    console.error("Error with changing spray status:", error.response?.data || error.message);
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
