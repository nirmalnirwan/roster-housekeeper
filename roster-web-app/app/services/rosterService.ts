import { Roster, RosterTask } from '../../lib/types';
import { apiClient } from './apiClient';

// wrapper functions that delegate to apiClient which automatically
// attaches the JWT token (when present) and handles base URL.

export async function getRosters(): Promise<Roster[]> {
  const res = await apiClient.get('/rosters');
  return res.json();
}

export async function getRoster(id: number): Promise<Roster> {
  const res = await apiClient.get(`/rosters/${id}`);
  return res.json();
}

export async function createRoster(roster: Roster): Promise<Roster> {
  const res = await apiClient.post('/rosters', roster);
  return res.json();
}

export async function updateRoster(id: number, roster: Roster): Promise<void> {
  await apiClient.put(`/rosters/${id}`, roster);
}

export async function deleteRoster(id: number): Promise<void> {
  await apiClient.delete(`/rosters/${id}`);
}

export async function exportRosterPdf(id: number): Promise<Blob> {
  const res = await apiClient.get(`/rosters/${id}/export/pdf`);
  return res.blob();
}

export async function exportRosterExcel(id: number): Promise<Blob> {
  const res = await apiClient.get(`/rosters/${id}/export/excel`);
  return res.blob();
}