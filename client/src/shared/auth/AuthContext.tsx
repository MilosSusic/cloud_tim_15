import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import { getUserRoleFromToken } from './utils';
import { login as apiLogin } from '../../features/auth/api/authApi';
import type { AuthContextType } from '../../features/auth/types/AuthContext';
import { setAxiosAuthToken } from '../api/axiosSetup';

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [token, setToken] = useState<string | null>(null);
  const [role, setRole] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const storedToken = localStorage.getItem('token');
    setToken(storedToken);
    setRole(getUserRoleFromToken(storedToken));
    setLoading(false);
  }, []);

  useEffect(() => {
    setAxiosAuthToken(token);
  }, [token]);

  const login = async (email: string, password: string) => {
    try {
      const response = await apiLogin({ email, password });
      const newToken = response.data;
      localStorage.setItem('token', newToken);
      setToken(newToken);
      setRole(getUserRoleFromToken(newToken));
      return true;
    } catch (error) {
      return false;
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    setToken(null);
    setRole(null);
  };

  return (
    <AuthContext.Provider value={{ token, role, isAuthenticated: !!token, loading, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
