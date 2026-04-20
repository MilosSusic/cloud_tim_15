import { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import axios from 'axios';
import { activateAccount } from '../api/authApi';

export default function ActivatePage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const token = searchParams.get('token') || '';

  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [message, setMessage] = useState('');
  const [isSuccess, setIsSuccess] = useState(false);
  const [loading, setLoading] = useState(false);

  const handleActivate = async () => {
    setMessage('');

    if (!token) {
      setMessage('Activation token is missing. Please use the activation link from your email.');
      return;
    }

    setLoading(true);

    try {
      await activateAccount({
        activationToken: token,
        password,
        confirmPassword,
      });

      setIsSuccess(true);
      setMessage('Account activated successfully. Redirecting to login...');

      setTimeout(() => navigate('/login'), 2500);
    } catch (error) {
      let apiMessage = 'Activation failed. Please try again.';

      if (axios.isAxiosError(error) && typeof error.response?.data === 'string') {
        apiMessage = error.response.data;
      }

      setMessage(apiMessage);
      setIsSuccess(false);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={styles.bg}>
      <div style={styles.card}>
        <div style={styles.logoRow}>
          <svg width="48" height="48" viewBox="0 0 75 75" xmlns="http://www.w3.org/2000/svg">
            <polygon
              points="37.5,5 67,20 67,55 37.5,70 8,55 8,20"
              fill="#ffb300"
              stroke="#e65100"
              strokeWidth="2"
            />
            <text x="37.5" y="45" textAnchor="middle" fontSize="22" fill="#333" fontWeight="bold">
              🐝
            </text>
          </svg>
          <span style={styles.logoText}>Smart Apiary</span>
        </div>
        <h2 style={styles.title}>Activate Account</h2>
        <p style={styles.subtitle}>Set your password to activate your account</p>
        {!token && (
          <div style={styles.info}>
            Open this page using the activation link that contains your token.
          </div>
        )}
        {message && (
          <div style={isSuccess ? styles.success : styles.error}>
            {message}
          </div>
        )}
        <div style={styles.field}>
          <label style={styles.label}>New Password</label>
          <input
            style={styles.input}
            type="password"
            placeholder="••••••••"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>
        <div style={styles.field}>
          <label style={styles.label}>Confirm Password</label>
          <input
            style={styles.input}
            type="password"
            placeholder="••••••••"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
          />
        </div>
        <button
          style={{ ...styles.button, opacity: loading ? 0.7 : 1 }}
          onClick={handleActivate}
          disabled={loading}
        >
          {loading ? 'Activating...' : 'Activate Account'}
        </button>
        <p style={styles.backLink} onClick={() => navigate('/login')}>
          ← Back to Login
        </p>
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
  info: {
    background: 'rgba(255,179,0,0.12)',
    border: '1px solid rgba(255,179,0,0.35)',
    borderRadius: '10px',
    color: '#ffd166',
    padding: '12px 16px',
    fontSize: '14px',
    marginBottom: '20px',
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
  success: {
    background: 'rgba(80,255,120,0.15)',
    border: '1px solid rgba(80,255,120,0.4)',
    borderRadius: '10px',
    color: '#6bffa0',
    padding: '16px',
    fontSize: '15px',
    textAlign: 'center',
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
    marginTop: '8px',
  },
  backLink: {
    color: '#ffb300',
    fontSize: '13px',
    cursor: 'pointer',
    textAlign: 'center',
    marginTop: '20px',
    textDecoration: 'underline',
  },
};
