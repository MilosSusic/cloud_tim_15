import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import type { User, CreateUserForm } from '../types/User';

const api = axios.create({ baseURL: 'https://localhost:7159/api' });
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export default function UserManagementPage() {
  const navigate = useNavigate();
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [actionLoading, setActionLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [search, setSearch] = useState('');
  const [form, setForm] = useState<CreateUserForm>({
    firstName: '',
    lastName: '',
    email: '',
    role: 2,
    phoneNumber: '',
  });

  useEffect(() => {
    const loadUsers = async () => {
      try {
        const response = await api.get<User[]>('/users');
        setUsers(response.data);
      } catch (err) {
        setError('Failed to load users.');
      } finally {
        setLoading(false);
      }
    };
    loadUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      const response = await api.get<User[]>('/users');
      setUsers(response.data);
    } catch {
      setError('Failed to load users.');
    }
  };

  const handleCreateUser = async () => {
    setError('');
    setActionLoading(true);
    try {
      await api.post('/users', form);
      setSuccess('User created! Activation email sent.');
      setShowModal(false);
      setForm({ firstName: '', lastName: '', email: '', role: 2, phoneNumber: '' });
      fetchUsers();
      setTimeout(() => setSuccess(''), 3000);
    } catch (err: unknown) {
      const e = err as { response?: { data?: string } };
      setError(e.response?.data || 'Failed to create user.');
    } finally {
      setActionLoading(false);
    }
  };

  const handleSuspend = async (email: string) => {
    setActionLoading(true);
    try {
      await api.post('/users/suspend', { email });
      setSuccess('User suspended.');
      fetchUsers();
      setTimeout(() => setSuccess(''), 3000);
    } catch (err: unknown) {
      const e = err as { response?: { data?: string } };
      setError(e.response?.data || 'Failed to suspend user.');
    } finally {
      setActionLoading(false);
    }
  };

  const handleDelete = async (email: string) => {
    if (!window.confirm('Are you sure you want to delete this user?')) return;
    setActionLoading(true);
    try {
      await api.post('/users/delete', { email });
      setSuccess('User deleted.');
      fetchUsers();
      setTimeout(() => setSuccess(''), 3000);
    } catch (err: unknown) {
      const e = err as { response?: { data?: string } };
      setError(e.response?.data || 'Failed to delete user.');
    } finally {
      setActionLoading(false);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/login');
  };

  const getRoleName = (role: string) => {
    const r = String(role);
    if (r === '1') return 'Admin';
    if (r === '2') return 'Beekeeper';
    if (r === '3') return 'Farmer';
    return r;
  };

  const getRoleStyle = (role: string): React.CSSProperties => {
    const r = String(role);
    if (r === '1')
      return { ...styles.roleBadge, background: 'rgba(255,183,0,0.15)', color: '#ffb300', border: '1px solid rgba(255,183,0,0.3)' };
    if (r === '2')
      return { ...styles.roleBadge, background: 'rgba(33,150,243,0.15)', color: '#64b5f6', border: '1px solid rgba(33,150,243,0.3)' };
    return { ...styles.roleBadge, background: 'rgba(76,175,80,0.15)', color: '#81c784', border: '1px solid rgba(76,175,80,0.3)' };
  };

  const getStatusBadge = (user: User): { label: string; color: string } => {
    if (user.isSuspended) return { label: 'Suspended', color: '#ff9800' };
    if (!user.isActive) return { label: 'Pending', color: '#9e9e9e' };
    return { label: 'Active', color: '#4caf50' };
  };

  const filteredUsers = users.filter(u =>
    u.firstName.toLowerCase().includes(search.toLowerCase()) ||
    u.lastName.toLowerCase().includes(search.toLowerCase()) ||
    u.email.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div style={styles.bg}>
      {/* Navbar */}
      <div style={styles.navbar}>
        <div style={styles.navLeft}>
          <svg width="36" height="36" viewBox="0 0 75 75" xmlns="http://www.w3.org/2000/svg">
            <polygon points="37.5,5 67,20 67,55 37.5,70 8,55 8,20" fill="#ffb300" stroke="#e65100" strokeWidth="2"/>
            <text x="37.5" y="45" textAnchor="middle" fontSize="22" fill="#333" fontWeight="bold">🐝</text>
          </svg>
          <span style={styles.navTitle}>Smart Apiary</span>
        </div>
        <div style={styles.navRight}>
          <div style={styles.adminInfo}>
            <div style={styles.adminAvatar}>A</div>
            <div>
              <div style={styles.adminName}>Administrator</div>
              <div style={styles.adminRole}>Admin Panel</div>
            </div>
          </div>
          <button style={styles.logoutBtn} onClick={handleLogout}>
            ⎋ Logout
          </button>
        </div>
      </div>

      {/* Content */}
      <div style={styles.content}>

        {/* Header row */}
        <div style={styles.header}>
          <div>
            <h1 style={styles.pageTitle}>User Management</h1>
            <p style={styles.pageSubtitle}>Manage system users and their roles</p>
          </div>
          <div style={styles.headerRight}>
            <input
              style={styles.searchInput}
              type="text"
              placeholder="🔍  Search users..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
            <button style={styles.addBtn} onClick={() => setShowModal(true)}>
              + Add User
            </button>
          </div>
        </div>

        {error && <div style={styles.errorMsg}>{error}</div>}
        {success && <div style={styles.successMsg}>{success}</div>}

        {/* Table */}
        <div style={styles.tableWrapper}>
          {loading ? (
            <div style={styles.loadingText}>Loading users...</div>
          ) : filteredUsers.length === 0 ? (
            <div style={styles.loadingText}>No users found.</div>
          ) : (
            <table style={styles.table}>
              <thead>
                <tr>
                  {['First Name', 'Last Name', 'Email', 'Phone Number', 'Role', 'Status', 'Suspend', 'Delete'].map((h) => (
                    <th key={h} style={styles.th}>{h}</th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {filteredUsers.map((user) => {
                  const badge = getStatusBadge(user);
                  return (
                    <tr key={user.id} style={styles.tr}>
                      <td style={styles.td}>
                        <span style={styles.userName}>{user.firstName}</span>
                      </td>
                      <td style={styles.td}>
                        <span style={styles.userName}>{user.lastName}</span>
                      </td>
                      <td style={styles.td}>
                        <span style={styles.userEmail}>{user.email}</span>
                      </td>
                      <td style={styles.td}>
                        <span style={styles.userEmail}>{user.phoneNumber}</span>
                      </td>
                      <td style={styles.td}>
                        <span style={getRoleStyle(String(user.role))}>
                          {getRoleName(String(user.role))}
                        </span>
                      </td>
                      <td style={styles.td}>
                        <span style={{
                          ...styles.statusBadge,
                          background: badge.color + '22',
                          color: badge.color,
                          border: `1px solid ${badge.color}55`,
                        }}>
                          {badge.label}
                        </span>
                      </td>
                      <td style={styles.td}>
                        {!user.isSuspended && user.isActive ? (
                          <button
                            style={styles.iconBtn}
                            title="Suspend user"
                            onClick={() => handleSuspend(user.email)}
                            disabled={actionLoading}
                          >
                            ⏸
                          </button>
                        ) : (
                          <span style={styles.disabledIcon}>⏸</span>
                        )}
                      </td>
                      <td style={styles.td}>
                        <button
                          style={{ ...styles.iconBtn, ...styles.deleteIconBtn }}
                          title="Delete user"
                          onClick={() => handleDelete(user.email)}
                          disabled={actionLoading}
                        >
                          🗑
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          )}
        </div>
      </div>

      {/* Modal */}
      {showModal && (
        <div style={styles.modalOverlay} onClick={() => setShowModal(false)}>
          <div style={styles.modal} onClick={(e) => e.stopPropagation()}>
            <div style={styles.modalHeader}>
              <h2 style={styles.modalTitle}>Add New User</h2>
              <button style={styles.closeBtn} onClick={() => setShowModal(false)}>✕</button>
            </div>
            {error && <div style={styles.errorMsg}>{error}</div>}
            <div style={styles.field}>
              <label style={styles.label}>First Name</label>
              <input
                style={styles.input}
                type="text"
                placeholder="John"
                value={form.firstName}
                onChange={(e) => setForm({ ...form, firstName: e.target.value })}
              />
            </div>
            <div style={styles.field}>
              <label style={styles.label}>Last Name</label>
              <input
                style={styles.input}
                type="text"
                placeholder="Doe"
                value={form.lastName}
                onChange={(e) => setForm({ ...form, lastName: e.target.value })}
              />
            </div>
            <div style={styles.field}>
              <label style={styles.label}>Email Address</label>
              <input
                style={styles.input}
                type="email"
                placeholder="john@example.com"
                value={form.email}
                onChange={(e) => setForm({ ...form, email: e.target.value })}
              />
            </div>
            <div style={styles.field}>
              <label style={styles.label}>Phone Number</label>
              <input
                style={styles.input}
                type="text"
                placeholder="061234567"
                maxLength={32}
                value={form.phoneNumber}
                onChange={(e) => setForm({ ...form, phoneNumber: e.target.value })}
              />
            </div>
            <div style={styles.field}>
              <label style={styles.label}>Role</label>
              <select
                style={styles.select}
                value={form.role}
                onChange={(e) => setForm({ ...form, role: parseInt(e.target.value) })}
              >
                <option value={2} style={{ background: '#1a1a2e', color: '#ffffff' }}>Beekeeper</option>
                <option value={3} style={{ background: '#1a1a2e', color: '#ffffff' }}>Farmer</option>
                <option value={1} style={{ background: '#1a1a2e', color: '#ffffff' }}>Admin</option>
              </select>
            </div>
            <div style={styles.modalActions}>
              <button style={styles.cancelBtn} onClick={() => setShowModal(false)}>
                Cancel
              </button>
              <button
                style={{ ...styles.createBtn, opacity: actionLoading ? 0.7 : 1 }}
                onClick={handleCreateUser}
                disabled={actionLoading}
              >
                {actionLoading ? 'Creating...' : 'Create User'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

const styles: { [key: string]: React.CSSProperties } = {
  bg: {
    minHeight: '100vh',
    background: 'linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%)',
    fontFamily: "'Segoe UI', sans-serif",
  },
  navbar: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: '14px 32px',
    background: 'rgba(255,255,255,0.04)',
    borderBottom: '1px solid rgba(255,183,0,0.2)',
    backdropFilter: 'blur(10px)',
  },
  navLeft: {
    display: 'flex',
    alignItems: 'center',
    gap: '12px',
  },
  navTitle: {
    color: '#ffb300',
    fontSize: '20px',
    fontWeight: '700',
    letterSpacing: '0.3px',
  },
  navRight: {
    display: 'flex',
    alignItems: 'center',
    gap: '20px',
  },
  adminInfo: {
    display: 'flex',
    alignItems: 'center',
    gap: '10px',
  },
  adminAvatar: {
    width: '36px',
    height: '36px',
    borderRadius: '50%',
    background: 'linear-gradient(135deg, #ffb300, #e65100)',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    color: '#fff',
    fontWeight: '700',
    fontSize: '15px',
  },
  adminName: {
    color: '#ffffff',
    fontSize: '14px',
    fontWeight: '600',
  },
  adminRole: {
    color: 'rgba(255,255,255,0.4)',
    fontSize: '11px',
  },
  logoutBtn: {
    padding: '8px 18px',
    background: 'rgba(255,80,80,0.12)',
    border: '1px solid rgba(255,80,80,0.35)',
    borderRadius: '8px',
    color: '#ff6b6b',
    fontSize: '13px',
    cursor: 'pointer',
    fontWeight: '600',
  },
  content: {
    padding: '32px',
    maxWidth: '1200px',
    margin: '0 auto',
  },
  header: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '28px',
  },
  pageTitle: {
    color: '#ffffff',
    fontSize: '26px',
    fontWeight: '700',
    margin: '0 0 4px 0',
  },
  pageSubtitle: {
    color: 'rgba(255,255,255,0.45)',
    fontSize: '14px',
    margin: 0,
  },
  headerRight: {
    display: 'flex',
    alignItems: 'center',
    gap: '12px',
  },
  searchInput: {
    padding: '11px 16px',
    background: 'rgba(255,255,255,0.07)',
    border: '1px solid rgba(255,255,255,0.12)',
    borderRadius: '10px',
    color: '#ffffff',
    fontSize: '14px',
    outline: 'none',
    width: '230px',
  },
  addBtn: {
    padding: '11px 22px',
    background: 'linear-gradient(135deg, #ffb300, #e65100)',
    border: 'none',
    borderRadius: '10px',
    color: '#fff',
    fontSize: '14px',
    fontWeight: '700',
    cursor: 'pointer',
    whiteSpace: 'nowrap' as const,
  },
  errorMsg: {
    background: 'rgba(255,80,80,0.12)',
    border: '1px solid rgba(255,80,80,0.35)',
    borderRadius: '10px',
    color: '#ff6b6b',
    padding: '12px 16px',
    fontSize: '14px',
    marginBottom: '16px',
  },
  successMsg: {
    background: 'rgba(80,255,120,0.12)',
    border: '1px solid rgba(80,255,120,0.35)',
    borderRadius: '10px',
    color: '#6bffa0',
    padding: '12px 16px',
    fontSize: '14px',
    marginBottom: '16px',
  },
  tableWrapper: {
    background: 'rgba(255,255,255,0.04)',
    borderRadius: '16px',
    border: '1px solid rgba(255,255,255,0.08)',
    overflow: 'hidden',
  },
  loadingText: {
    color: 'rgba(255,255,255,0.4)',
    padding: '48px',
    textAlign: 'center',
    fontSize: '15px',
  },
  table: {
    width: '100%',
    borderCollapse: 'collapse',
  },
  th: {
    color: '#ffb300',
    fontWeight: '700',
    fontSize: '14px',
    padding: '16px 8px',
    background: 'rgba(255,255,255,0.03)',
    borderBottom: '1px solid rgba(255,255,255,0.08)',
    textAlign: 'left' as const,
  },
  tr: {
    borderBottom: '1px solid rgba(255,255,255,0.06)',
  },
  td: {
    color: '#fff',
    fontSize: '14px',
    padding: '14px 8px',
    verticalAlign: 'middle',
  },
  userName: {
    fontWeight: '600',
    color: '#fff',
  },
  userEmail: {
    color: '#ffd166',
    fontSize: '13px',
  },
  roleBadge: {
    display: 'inline-block',
    padding: '4px 12px',
    borderRadius: '8px',
    fontWeight: '600',
    fontSize: '13px',
    marginRight: '4px',
  },
  statusBadge: {
    display: 'inline-block',
    padding: '4px 12px',
    borderRadius: '8px',
    fontWeight: '600',
    fontSize: '13px',
    marginRight: '4px',
  },
  iconBtn: {
    background: 'none',
    border: 'none',
    color: '#ffb300',
    fontSize: '18px',
    cursor: 'pointer',
    padding: '4px 8px',
    borderRadius: '6px',
    transition: 'background 0.2s',
  },
  deleteIconBtn: {
    color: '#ff4444',
  },
  disabledIcon: {
    color: '#aaa',
    fontSize: '18px',
    padding: '4px 8px',
  },
  modalOverlay: {
    position: 'fixed' as const,
    top: 0,
    left: 0,
    width: '100vw',
    height: '100vh',
    background: 'rgba(0,0,0,0.45)',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    zIndex: 1000,
  },
  modal: {
    background: '#23234b',
    borderRadius: '16px',
    padding: '32px 28px',
    minWidth: '340px',
    maxWidth: '95vw',
    boxShadow: '0 8px 32px 0 rgba(31,38,135,0.37)',
    position: 'relative' as const,
  },
  modalHeader: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    marginBottom: '18px',
  },
  modalTitle: {
    color: '#fff',
    fontSize: '20px',
    fontWeight: '700',
    margin: 0,
  },
  closeBtn: {
    background: 'none',
    border: 'none',
    color: '#ffb300',
    fontSize: '20px',
    cursor: 'pointer',
    padding: '2px 8px',
    borderRadius: '6px',
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
  select: {
    width: '100%',
    padding: '12px 16px',
    background: 'rgba(255,255,255,0.08)',
    border: '1px solid rgba(255,255,255,0.15)',
    borderRadius: '10px',
    color: '#ffffff',
    fontSize: '15px',
    outline: 'none',
    boxSizing: 'border-box',
    appearance: 'none',
  },
  modalActions: {
    display: 'flex',
    justifyContent: 'flex-end',
    gap: '12px',
    marginTop: '18px',
  },
  cancelBtn: {
    padding: '10px 22px',
    background: 'rgba(255,255,255,0.08)',
    border: '1px solid rgba(255,255,255,0.15)',
    borderRadius: '10px',
    color: '#fff',
    fontSize: '14px',
    fontWeight: '600',
    cursor: 'pointer',
  },
  createBtn: {
    padding: '10px 22px',
    background: 'linear-gradient(135deg, #ffb300, #e65100)',
    border: 'none',
    borderRadius: '10px',
    color: '#fff',
    fontSize: '14px',
    fontWeight: '700',
    cursor: 'pointer',
  },
};
