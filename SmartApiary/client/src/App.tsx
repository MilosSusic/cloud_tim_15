import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from './features/auth/pages/LoginPage';
import ActivatePage from './features/auth/pages/ActivatePage';
import ForgotPasswordPage from './features/auth/pages/ForgotPasswordPage';
import ResetPasswordPage from './features/auth/pages/ResetPasswordPage';
import UserManagementPage from './features/userManagement/pages/UserManagementPage';
import AccessDeniedPage from './features/auth/pages/AccessDeniedPage';
import PrivateRoute from './features/auth/components/PrivateRoute';
import { AuthProvider } from './features/auth/context/AuthContext';

import BeehivesPage from './features/apiaries/pages/BeehivesPage';

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/activate" element={<ActivatePage />} />
          <Route path="/forgot-password" element={<ForgotPasswordPage />} />
          <Route path="/reset-password" element={<ResetPasswordPage />} />
          <Route path="/access-denied" element={<AccessDeniedPage />} />

          <Route
            path="/beekeeper/apiaries"
            element={
              <PrivateRoute requiredRole="Beekeeper">
                <BeehivesPage />
              </PrivateRoute>
            }
          />

          <Route
            path="/admin/users"
            element={
              <PrivateRoute requiredRole="Admin">
                <UserManagementPage />
              </PrivateRoute>
            }
          />
          <Route path="*" element={<Navigate to="/login" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;