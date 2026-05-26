// authentication helper for traditional JWT login
import { LoginRequest, LoginResponse, User } from '../../types/auth';

const API_BASE = process.env.NEXT_PUBLIC_API_BASE || 'http://localhost:5000';
// authentication service may run on its own port, default to 5001
const AUTH_BASE = process.env.NEXT_PUBLIC_AUTH_BASE || API_BASE;
const TOKEN_KEY = 'token';
const REFRESH_TOKEN_KEY = 'refreshToken';
const USER_KEY = 'user';

export async function login(username: string, password: string): Promise<LoginResponse> {
  const body: LoginRequest = { username, password };
  const res = await fetch(`${AUTH_BASE}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email: username, password }), // backend expects email property
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.error || 'Login failed');
  }
  const data = await res.json();
  // map API response to our simplified interface
  const mapped: LoginResponse = {
    token: data.accessToken || data.token,
    refreshToken: data.refreshToken,
    user: {
      id: data.user?.id,
      username: data.user?.email || data.user?.username,
      role: (Array.isArray(data.user?.roles) ? data.user.roles[0] : data.user?.role) || '',
    },
  };
  setToken(mapped.token);
  setRefreshToken(mapped.refreshToken);
  setUser(mapped.user);
  return mapped;
}

export async function refresh(): Promise<LoginResponse> {
  const stored = typeof window !== 'undefined' ? localStorage.getItem(REFRESH_TOKEN_KEY) : null;
  if (!stored) throw new Error('No refresh token');
  const res = await fetch(`${AUTH_BASE}/api/auth/refresh-token`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken: stored }),
  });
  if (!res.ok) {
    logout();
    throw new Error('Refresh failed');
  }
  const data = await res.json();
  const mapped: LoginResponse = {
    token: data.accessToken || data.token,
    refreshToken: data.refreshToken,
    user: getCurrentUser() as User, // preserve existing user info
  };
  setToken(mapped.token);
  setRefreshToken(mapped.refreshToken);
  return mapped;
}

export function logout(): void {
  clearStorage();
}

export function getToken(): string | null {
  if (typeof window === 'undefined') return null;
  return localStorage.getItem(TOKEN_KEY);
}

export function isAuthenticated(): boolean {
  return !!getToken();
}

export function getCurrentUser(): User | null {
  if (typeof window === 'undefined') return null;
  const u = localStorage.getItem(USER_KEY);
  if (!u) return null;
  try {
    return JSON.parse(u);
  } catch {
    return null;
  }
}

function setToken(token: string) {
  if (typeof window !== 'undefined') {
    localStorage.setItem(TOKEN_KEY, token);
    // also set cookie so middleware or server code can read if needed
    document.cookie = `token=${token}; path=/`;
  }
}

function setRefreshToken(token: string) {
  if (typeof window !== 'undefined') {
    localStorage.setItem(REFRESH_TOKEN_KEY, token);
  }
}

function setUser(user: User) {
  if (typeof window !== 'undefined') {
    localStorage.setItem(USER_KEY, JSON.stringify(user));
  }
}

function clearStorage() {
  if (typeof window !== 'undefined') {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    document.cookie = 'token=;path=/; max-age=0';
  }
}
