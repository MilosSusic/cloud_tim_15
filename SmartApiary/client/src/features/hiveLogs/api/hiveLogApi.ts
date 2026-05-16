import axios from 'axios';
import { getToken } from '../../auth/api/authApi';
import { CONFIG } from '../../../config/config';
import type {
  HiveLogEntryDto,
  PagedResult,
  CreateHiveLogEntryRequest,
  UpdateHiveLogEntryRequest,
} from '../types/HiveLogEntry';

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

export async function getLogsByHive(
  hiveId: string,
  page = 1,
  pageSize = 10
): Promise<PagedResult<HiveLogEntryDto>> {
  try {
    const response = await axios.get(`${CONFIG.HIVE_LOG_ENTRIES_URL}/by-hive/${hiveId}`, {
      params: { page, pageSize },
      headers: authHeaders(),
    });
    return response.data;
  } catch (error) {
    throw new Error(extractAxiosErrorMessage(error, 'Failed to fetch log entries.'));
  }
}

export async function createLogEntry(
  payload: CreateHiveLogEntryRequest
): Promise<string> {
  try {
    const response = await axios.post(CONFIG.HIVE_LOG_ENTRIES_URL, payload, {
      headers: authJsonHeaders(),
    });
    return typeof response.data === 'string' ? response.data : '';
  } catch (error) {
    throw new Error(extractAxiosErrorMessage(error, 'Failed to create log entry.'));
  }
}

export async function updateLogEntry(
  id: string,
  payload: UpdateHiveLogEntryRequest
): Promise<void> {
  try {
    await axios.put(`${CONFIG.HIVE_LOG_ENTRIES_URL}/${id}`, payload, {
      headers: authJsonHeaders(),
    });
  } catch (error) {
    throw new Error(extractAxiosErrorMessage(error, 'Failed to update log entry.'));
  }
}

export async function deleteLogEntry(id: string): Promise<void> {
  try {
    await axios.delete(`${CONFIG.HIVE_LOG_ENTRIES_URL}/${id}`, {
      headers: authHeaders(),
    });
  } catch (error) {
    throw new Error(extractAxiosErrorMessage(error, 'Failed to delete log entry.'));
  }
}
