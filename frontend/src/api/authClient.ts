import type { AuthResponse, LoginCredentials, MeResponse } from '../types/auth';
import { getAccessToken } from '../utils/authStorage';

const baseUrl = import.meta.env.VITE_API_BASE_URL;

if (!baseUrl) {
  throw new Error('VITE_API_BASE_URL is not set');
}

async function readErrorMessage(response: Response): Promise<string> {
  const text = await response.text();
  if (!text) return `Request failed (${response.status})`;
  try {
    const body = JSON.parse(text) as { error?: string; title?: string };
    return body.error ?? body.title ?? text;
  } catch {
    return text;
  }
}

export async function login(credentials: LoginCredentials): Promise<AuthResponse> {
  const response = await fetch(`${baseUrl}/api/v1/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(credentials),
  });

  if (!response.ok) {
    throw new Error(await readErrorMessage(response));
  }

  return response.json() as Promise<AuthResponse>;
}

export async function getMe(signal?: AbortSignal): Promise<MeResponse> {
  const token = getAccessToken();
  if (!token) {
    throw new Error('Not logged in.');
  }

  const response = await fetch(`${baseUrl}/api/v1/auth/me`, {
    headers: { Authorization: `Bearer ${token}` },
    signal,
  });

  if (response.status === 401) {
    throw new Error('Session expired. Please log in again.');
  }

  if (!response.ok) {
    throw new Error(await readErrorMessage(response));
  }

  return response.json() as Promise<MeResponse>;
}

export async function register(credentials: LoginCredentials): Promise<AuthResponse> {
    const response = await fetch(`${baseUrl}/api/v1/auth/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(credentials),
    });
    if (!response.ok) throw new Error(await readErrorMessage(response));
    return response.json() as Promise<AuthResponse>;
  }