import type { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import { getToken, getUserRoleFromToken } from '../api/authApi';

interface PrivateRouteProps {
  children: ReactNode;
  requiredRole?: string;
}

export default function PrivateRoute({
  children,
  requiredRole,
}: PrivateRouteProps) {
  const token = getToken();

  if (!token) {
    return <Navigate to="/login" replace />;
  }

  if (requiredRole) {
    const role = getUserRoleFromToken();

    if (role !== requiredRole) {
      return <Navigate to="/access-denied" replace />;
    }
  }

  return <>{children}</>;
}