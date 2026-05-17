import axios from 'axios';
import type {
  LoginRequest,
  ActivateAccountRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
} from '../types/AuthRequests';
import { CONFIG } from '../../../shared/config/config';

// Auth API calls - koristi globalni axios sa interceptorima
export const login = (data: LoginRequest) =>
  axios.post<string>(CONFIG.AUTH_LOGIN_URL, data);

export const activateAccount = (data: ActivateAccountRequest) =>
  axios.post(CONFIG.AUTH_ACTIVATE_URL, data);

export const forgotPassword = (data: ForgotPasswordRequest) =>
  axios.post(CONFIG.AUTH_FORGOT_PASSWORD_URL, data);

export const resetPassword = (data: ResetPasswordRequest) =>
  axios.post(CONFIG.AUTH_RESET_PASSWORD_URL, data);
