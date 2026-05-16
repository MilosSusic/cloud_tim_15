import axios from 'axios';
import { getToken } from '../../auth/api/authApi';
import { CONFIG } from '../../../config/config';
import type { Hive } from '../types/Hive';

const authJsonHeaders = () => ({
  'Content-Type': 'application/json',
  Authorization: `Bearer ${getToken()}`,
});

const authHeaders = () => ({
  Authorization: `Bearer ${getToken()}`,
});

const extractAxiosErrorMessage = (error: unknown, fallback: string): string => {
  if (axios.isAxiosError(error)) {
    const apiMessage = error.response?.data;
    if (typeof apiMessage === 'string' && apiMessage.trim()) {
      return apiMessage;
    }
  }
  return fallback;
};

export async function getHivesByApiary(apiaryId: string): Promise<Hive[]> {
  try {
    const response = await axios.get(`${CONFIG.HIVES_URL}/by-apiary/${apiaryId}`, {
      headers: authHeaders(),
    });
    return response.data;
  } catch (error) {
    throw new Error(extractAxiosErrorMessage(error, 'Failed to fetch hives'));
  }
}

export async function createHive(payload: {
  apiaryId: string;
  label: string;
  type: string;
  color: string;
  queenAge: number;
  note?: string | null;
}): Promise<string> {
  try {
    const response = await axios.post(CONFIG.HIVES_URL, payload, {
      headers: authJsonHeaders(),
    });
    return typeof response.data === 'string' ? response.data : '';
  } catch (error) {
    throw new Error(extractAxiosErrorMessage(error, 'Failed to create hive'));
  }
}

export async function updateHive(
  id: string,
  payload: {
    id: string;
    label: string;
    type: string;
    color: string;
    queenAge: number;
    note?: string | null;
  }
): Promise<void> {
  try {
    await axios.put(`${CONFIG.HIVES_URL}/${id}`, payload, {
      headers: authJsonHeaders(),
    });
  } catch (error) {
    throw new Error(extractAxiosErrorMessage(error, 'Failed to update hive'));
  }
}

export async function deleteHive(id: string): Promise<void> {
  try {
    await axios.delete(`${CONFIG.HIVES_URL}/${id}`, {
      headers: authHeaders(),
    });
  } catch (error) {
    throw new Error(extractAxiosErrorMessage(error, 'Failed to delete hive'));
  }
}
