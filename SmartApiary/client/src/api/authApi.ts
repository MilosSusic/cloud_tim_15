import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7159/api',
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export interface LoginRequest {
  email: string;
  password: string;
}

export interface ActivateAccountRequest {
  activationToken: string;
  password: string;
  confirmPassword: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  resetPasswordToken: string;
  password: string;
  confirmPassword: string;
}

export const login = (data: LoginRequest) =>
  api.post<string>('/auth/login', data);

export const activateAccount = (data: ActivateAccountRequest) =>
  api.post('/auth/activate', data);

export const forgotPassword = (data: ForgotPasswordRequest) =>
  api.post('/auth/forgot-password', data);

export const resetPassword = (data: ResetPasswordRequest) =>
  api.post('/auth/reset-password', data);

export function getToken(): string | null {
  return localStorage.getItem('token');
}

export function getUserRoleFromToken(): string | null {
  const token = getToken();
  if (!token) return null;

  try {
    const payload = JSON.parse(atob(token.split('.')[1])) as Record<string, string>;

    return (
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
      payload['role'] ||
      null
    );
  } catch {
    return null;
  }
}