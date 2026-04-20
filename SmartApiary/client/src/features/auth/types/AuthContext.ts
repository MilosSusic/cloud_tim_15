// Auth context type for Auth feature
export interface AuthContextType {
  token: string | null;
  role: string | null;
  isAuthenticated: boolean;
  loading: boolean;
  login: (email: string, password: string) => Promise<boolean>;
  logout: () => void;
}
