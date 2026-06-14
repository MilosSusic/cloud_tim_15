export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: number;
  isActive: boolean;
  isSuspended: boolean;
  phoneNumber: string;
}

export interface CreateUserForm {
  firstName: string;
  lastName: string;
  email: string;
  role: number;
  phoneNumber: string;
}
