import axios from 'axios';
export const setAxiosAuthToken = (token: string | null) => {
  if (token) {
    axios.defaults.headers.common.Authorization = `Bearer ${token}`;
    return;
  }

  delete axios.defaults.headers.common.Authorization;
};

// Response interceptor - centralizovani error handling
axios.interceptors.response.use(
  (response) => response,
  (error) => {
    // Redirect na login ako je 401 (Unauthorized)
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      delete axios.defaults.headers.common.Authorization;
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default axios;
