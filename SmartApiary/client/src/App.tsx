import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import LoginPage from "./features/auth/pages/LoginPage";
import ActivatePage from "./features/auth/pages/ActivatePage";
import ForgotPasswordPage from "./features/auth/pages/ForgotPasswordPage";
import ResetPasswordPage from "./features/auth/pages/ResetPasswordPage";
import UserManagementPage from "./features/userManagement/pages/UserManagementPage";
import AccessDeniedPage from "./features/auth/pages/AccessDeniedPage";
import PrivateRoute from "./features/auth/components/PrivateRoute";
import { AuthProvider } from "./shared/auth/AuthContext";
import { SignalRProvider } from "./shared/context/SignalRContext";

import ApiaryPage from "./features/apiaries/pages/ApiaryPage";
//import ApiaryAnalyticsPage from "./features/apiaries/pages/ApiaryAnalyticsPage";
import FieldPage from "./features/fields/pages/FieldPage";
import SprayLogPage from "./features/spraySchedule/pages/SprayLogPage";

function App() {
  return (
    <AuthProvider>
      <SignalRProvider>
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
                  <ApiaryPage />
                </PrivateRoute>
              }
            />

            {/* <Route
              path="/beekeeper/analytics"
              element={
                <PrivateRoute requiredRole="Beekeeper">
                  <ApiaryAnalyticsPage />
                </PrivateRoute>
              }
            /> */}

            <Route
              path="/farmer/field"
              element={
                <PrivateRoute requiredRole="Farmer">
                  <FieldPage />
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
            <Route
              path="/farmer/spray-logs"
              element={
                <PrivateRoute requiredRole="Farmer">
                  <SprayLogPage />
                </PrivateRoute>
              }
            />
            <Route path="*" element={<Navigate to="/login" replace />} />
          </Routes>
        </BrowserRouter>
      </SignalRProvider>
    </AuthProvider>
  );
}

export default App;
