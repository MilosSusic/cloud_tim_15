import axios from 'axios';

type ValidationErrors = Record<string, string[] | string>;

export const extractAxiosErrorMessage = (error: unknown, fallback: string): string => {
  if (!axios.isAxiosError(error)) {
    return fallback;
  }

  const payload = error.response?.data;

  if (typeof payload === 'string' && payload.trim()) {
    return payload;
  }

  if (payload && typeof payload === 'object') {
    const data = payload as {
      message?: unknown;
      title?: unknown;
      errors?: ValidationErrors;
    };

    if (typeof data.message === 'string' && data.message.trim()) {
      return data.message;
    }

    if (typeof data.title === 'string' && data.title.trim()) {
      return data.title;
    }

    if (data.errors && typeof data.errors === 'object') {
      const first = Object.values(data.errors)[0];
      if (Array.isArray(first) && first.length > 0) {
        return String(first[0]);
      }
      if (typeof first === 'string' && first.trim()) {
        return first;
      }
    }
  }

  return fallback;
};
