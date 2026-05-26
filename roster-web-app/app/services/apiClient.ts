import { getToken } from './authService';

const API_BASE = process.env.NEXT_PUBLIC_API_BASE || 'http://localhost:5000';

async function request(path: string, options: RequestInit = {}) {
  const token = getToken();
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(options.headers as Record<string, string>),
  };
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  let res = await fetch(`${API_BASE}${path}`, {
    ...options,
    headers,
  });

  // try refreshing once if unauthorized
  if (res.status === 401) {
    try {
      const { refresh } = await import('./authService');
      const newTokens = await refresh();
      headers['Authorization'] = `Bearer ${newTokens.token}`;
      res = await fetch(`${API_BASE}${path}`, {
        ...options,
        headers,
      });
    } catch (e) {
      // if refresh fails, propagate original error
    }
  }

  if (!res.ok) {
    // attempt to read body for error message
    let errorMsg = res.statusText;
    try {
      const body = await res.json();
      if (body && body.error) errorMsg = body.error;
    } catch {}
    throw new Error(errorMsg);
  }

  return res;
}

export const apiClient = {
  get: (path: string) => request(path, { method: 'GET' }),
  post: (path: string, body?: any) =>
    request(path, { method: 'POST', body: body ? JSON.stringify(body) : undefined }),
  put: (path: string, body?: any) =>
    request(path, { method: 'PUT', body: body ? JSON.stringify(body) : undefined }),
  delete: (path: string) => request(path, { method: 'DELETE' }),
};
