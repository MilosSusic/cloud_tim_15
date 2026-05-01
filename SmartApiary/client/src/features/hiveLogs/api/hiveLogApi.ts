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

export async function getLogsByHive(
  hiveId: string,
  page = 1,
  pageSize = 10
): Promise<PagedResult<HiveLogEntryDto>> {
  const url = `${CONFIG.HIVE_LOG_ENTRIES_URL}/by-hive/${hiveId}?page=${page}&pageSize=${pageSize}`;
  const resp = await fetch(url, { headers: authHeaders() });
  if (!resp.ok) {
    const err = await resp.text();
    throw new Error(err || 'Failed to fetch log entries.');
  }
  return resp.json();
}

export async function createLogEntry(
  payload: CreateHiveLogEntryRequest
): Promise<string> {
  const resp = await fetch(CONFIG.HIVE_LOG_ENTRIES_URL, {
    method: 'POST',
    headers: authJsonHeaders(),
    body: JSON.stringify(payload),
  });
  if (!resp.ok) {
    const err = await resp.text();
    throw new Error(err || 'Failed to create log entry.');
  }
  const text = await resp.text();
  return text ? JSON.parse(text) : '';
}

export async function updateLogEntry(
  id: string,
  payload: UpdateHiveLogEntryRequest
): Promise<void> {
  const resp = await fetch(`${CONFIG.HIVE_LOG_ENTRIES_URL}/${id}`, {
    method: 'PUT',
    headers: authJsonHeaders(),
    body: JSON.stringify(payload),
  });
  if (!resp.ok) {
    const err = await resp.text();
    throw new Error(err || 'Failed to update log entry.');
  }
}

export async function deleteLogEntry(id: string): Promise<void> {
  const resp = await fetch(`${CONFIG.HIVE_LOG_ENTRIES_URL}/${id}`, {
    method: 'DELETE',
    headers: authHeaders(),
  });
  if (!resp.ok) {
    const err = await resp.text();
    throw new Error(err || 'Failed to delete log entry.');
  }
}
