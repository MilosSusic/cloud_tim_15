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

export async function getHivesByApiary(apiaryId: string): Promise<Hive[]> {
  const resp = await fetch(`${CONFIG.HIVES_URL}/by-apiary/${apiaryId}`, {
    headers: authHeaders(),
  });
  if (!resp.ok) throw new Error('Greška pri dohvatanju košnica');
  return resp.json();
}

export async function createHive(payload: {
  apiaryId: string;
  label: string;
  type: string;
  color: string;
  queenAge: number;
  note?: string | null;
}): Promise<string> {
  const resp = await fetch(CONFIG.HIVES_URL, {
    method: 'POST',
    headers: authJsonHeaders(),
    body: JSON.stringify(payload),
  });
  if (!resp.ok) {
    const err = await resp.text();
    throw new Error(err || 'Greška pri kreiranju košnice');
  }
  const text = await resp.text();
  return text ? JSON.parse(text) : '';
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
  const resp = await fetch(`${CONFIG.HIVES_URL}/${id}`, {
    method: 'PUT',
    headers: authJsonHeaders(),
    body: JSON.stringify(payload),
  });
  if (!resp.ok) {
    const err = await resp.text();
    throw new Error(err || 'Greška pri izmeni košnice');
  }
}

export async function deleteHive(id: string): Promise<void> {
  const resp = await fetch(`${CONFIG.HIVES_URL}/${id}`, {
    method: 'DELETE',
    headers: authHeaders(),
  });
  if (!resp.ok) {
    const err = await resp.text();
    throw new Error(err || 'Greška pri brisanju košnice');
  }
}
