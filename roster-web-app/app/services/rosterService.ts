import { Roster, RosterTask } from '../../lib/types';

const API_BASE = process.env.NEXT_PUBLIC_API_BASE || 'http://localhost:5000';

export async function getRosters(): Promise<Roster[]> {
  const res = await fetch(`${API_BASE}/api/rosters`, {
    headers: { 'Content-Type': 'application/json' }
  });
  return res.json();
}

export async function getRoster(id: number): Promise<Roster> {
  const res = await fetch(`${API_BASE}/api/rosters/${id}`, {
    headers: { 'Content-Type': 'application/json' }
  });
  return res.json();
}

export async function createRoster(roster: Roster): Promise<Roster> {
  const res = await fetch(`${API_BASE}/api/rosters`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(roster)
  });
  return res.json();
}

export async function updateRoster(id: number, roster: Roster): Promise<void> {
  await fetch(`${API_BASE}/api/rosters/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(roster)
  });
}

export async function deleteRoster(id: number): Promise<void> {
  await fetch(`${API_BASE}/api/rosters/${id}`, { method: 'DELETE' });
}

export async function exportRosterPdf(id: number): Promise<Blob> {
  const res = await fetch(`${API_BASE}/api/rosters/${id}/export/pdf`);
  return res.blob();
}

export async function exportRosterExcel(id: number): Promise<Blob> {
  const res = await fetch(`${API_BASE}/api/rosters/${id}/export/excel`);
  return res.blob();
}