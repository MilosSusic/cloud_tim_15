
import type { PrivateRouteProps } from '../types/PrivateRoute';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function PrivateRoute({
  children,
  requiredRole,
}: PrivateRouteProps) {
  const { role, isAuthenticated, loading } = useAuth();

  if (loading) {
    // Prikaz loadera dok se učitava stanje autentifikacije
    return null; // ili <div>Loading...</div>
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (requiredRole && role !== requiredRole) {
    return <Navigate to="/access-denied" replace />;
  }

  return <>{children}</>;
}
