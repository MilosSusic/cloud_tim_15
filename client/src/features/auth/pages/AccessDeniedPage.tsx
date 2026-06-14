import { useNavigate } from 'react-router-dom';

export default function AccessDeniedPage() {
  const navigate = useNavigate();

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

        <h2 style={styles.title}>Access limited</h2>
        <p style={styles.subtitle}>
          Your account was authenticated successfully, but this dashboard is not part of the current implementation.
        </p>

        <button style={styles.button} onClick={() => navigate('/login')}>
          Back to Login
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
    padding: '40px 32px',
    boxShadow: '0 8px 32px 0 rgba(31,38,135,0.37)',
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    maxWidth: '400px',
    width: '100%',
  },
  logoRow: {
    display: 'flex',
    alignItems: 'center',
    gap: '12px',
    marginBottom: '24px',
  },
  logoText: {
    fontSize: '1.5rem',
    fontWeight: 700,
    color: '#ffb300',
    letterSpacing: '1px',
  },
  title: {
    fontSize: '1.4rem',
    fontWeight: 600,
    color: '#fff',
    marginBottom: '8px',
    marginTop: 0,
  },
  subtitle: {
    color: '#eee',
    fontSize: '1rem',
    marginBottom: '24px',
    textAlign: 'center',
  },
  button: {
    background: 'linear-gradient(90deg, #ffb300 0%, #e65100 100%)',
    color: '#fff',
    border: 'none',
    borderRadius: '8px',
    padding: '10px 24px',
    fontWeight: 600,
    fontSize: '1rem',
    cursor: 'pointer',
    marginTop: '16px',
    transition: 'background 0.2s',
  },
};
