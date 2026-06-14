// Auth request interfaces for Auth feature

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
