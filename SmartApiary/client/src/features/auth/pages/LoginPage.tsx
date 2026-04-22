import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { useAuth } from '../context/AuthContext';
import { getUserRoleFromToken } from '../api/authApi';

export default function LoginPage() {
  const navigate = useNavigate();
  const { login: authLogin } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleLogin = async () => {
    setError('');
    setLoading(true);

    try {
      const success = await authLogin(email, password);
      if (!success) throw new Error();

      // Pročitaj rolu direktno iz tokena odmah nakon logina
      const newRole = getUserRoleFromToken();

      if (newRole === 'Admin') {
        navigate('/admin/users');
      } else if (newRole === 'Beekeeper') {
        navigate('/beekeeper/apiaries');
      } else {
        navigate('/access-denied');
      }
    } catch (error) {
      let message = 'Login failed. Please try again.';

      if (axios.isAxiosError(error) && typeof error.response?.data === 'string') {
        message = error.response.data;
      }

      setError(message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={styles.bg}>
      <div style={styles.card}>
        <div style={styles.logoRow}>
          <svg width="48" height="48" viewBox="0 0 75 75" xmlns="http://www.w3.org/2000/svg">
            <polygon points="37.5,5 67,20 67,55 37.5,70 8,55 8,20" fill="#ffb300" stroke="#e65100" strokeWidth="2" />
            <text x="37.5" y="45" textAnchor="middle" fontSize="22" fill="#333" fontWeight="bold">🐝</text>
          </svg>
          <span style={styles.logoText}>Smart Apiary</span>
        </div>
        <h2 style={styles.title}>Welcome back</h2>
        <p style={styles.subtitle}>Sign in to your account</p>
        {error && <div style={styles.error}>{error}</div>}
        <div style={styles.field}>
          <label style={styles.label}>Email Address</label>
          <input
            style={styles.input}
            type="email"
            placeholder="you@example.com"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleLogin()}
          />
        </div>
        <div style={styles.field}>
          <label style={styles.label}>Password</label>
          <input
            style={styles.input}
            type="password"
            placeholder="••••••••"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleLogin()}
          />
        </div>
        <div style={styles.links}>
          <span style={styles.link} onClick={() => navigate('/forgot-password')}>
            Forgot Password?
          </span>
        </div>
        <button
          style={{ ...styles.button, opacity: loading ? 0.7 : 1 }}
          onClick={handleLogin}
          disabled={loading}
        >
          {loading ? 'Signing in...' : 'Login'}
        </button>
      </div>
    </div>
  );
}

const styles: { [key: string]: React.CSSProperties } = {
  bg: {
    minHeight: '100vh',
    background: 'linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%)',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    fontFamily: "'Segoe UI', sans-serif",
  },
  card: {
    background: 'rgba(255,255,255,0.05)',
    backdropFilter: 'blur(20px)',
    border: '1px solid rgba(255,183,0,0.2)',
    borderRadius: '20px',
    padding: '48px 40px',
    width: '100%',
    maxWidth: '420px',
    boxShadow: '0 25px 50px rgba(0,0,0,0.4)',
  },
  logoRow: {
    display: 'flex',
    alignItems: 'center',
    gap: '12px',
    marginBottom: '28px',
    justifyContent: 'center',
  },
  logoText: {
    fontSize: '22px',
    fontWeight: '700',
    color: '#ffb300',
    letterSpacing: '0.5px',
  },
  title: {
    color: '#ffffff',
    fontSize: '24px',
    fontWeight: '700',
    margin: '0 0 6px 0',
    textAlign: 'center',
  },
  subtitle: {
    color: 'rgba(255,255,255,0.5)',
    fontSize: '14px',
    textAlign: 'center',
    margin: '0 0 28px 0',
  },
  error: {
    background: 'rgba(255,80,80,0.15)',
    border: '1px solid rgba(255,80,80,0.4)',
    borderRadius: '10px',
    color: '#ff6b6b',
    padding: '12px 16px',
    fontSize: '14px',
    marginBottom: '20px',
  },
  field: {
    marginBottom: '18px',
  },
  label: {
    display: 'block',
    color: 'rgba(255,255,255,0.7)',
    fontSize: '13px',
    fontWeight: '600',
    marginBottom: '8px',
    letterSpacing: '0.3px',
  },
  input: {
    width: '100%',
    padding: '12px 16px',
    background: 'rgba(255,255,255,0.08)',
    border: '1px solid rgba(255,255,255,0.15)',
    borderRadius: '10px',
    color: '#ffffff',
    fontSize: '15px',
    outline: 'none',
    boxSizing: 'border-box',
    transition: 'border-color 0.2s',
  },
  links: {
    display: 'flex',
    justifyContent: 'flex-end',
    marginBottom: '24px',
  },
  link: {
    color: '#ffb300',
    fontSize: '13px',
    cursor: 'pointer',
    textDecoration: 'underline',
  },
  button: {
    width: '100%',
    padding: '14px',
    background: 'linear-gradient(135deg, #ffb300, #e65100)',
    border: 'none',
    borderRadius: '10px',
    color: '#fff',
    fontSize: '16px',
    fontWeight: '700',
    cursor: 'pointer',
    letterSpacing: '0.5px',
  },
};
