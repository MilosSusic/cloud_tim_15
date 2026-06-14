export interface ApiaryFormErrors {
  name?: string;
  description?: string;
  latitude?: string;
  longitude?: string;
}

interface ApiaryFormFields {
  name: string;
  description: string;
  latitude: string;
  longitude: string;
}

export function useApiaryValidation(form: ApiaryFormFields): {
  errors: ApiaryFormErrors;
  isValid: boolean;
} {
  const errors: ApiaryFormErrors = {};

  if (!form.name.trim()) {
    errors.name = "Name is required.";
  } else if (form.name.length > 100) {
    errors.name = "Max 100 characters.";
  }

  if (form.description && form.description.length > 512) {
    errors.description = "Max 512 characters.";
  }

  if (!form.latitude.trim()) {
    errors.latitude = "Latitude is required.";
  } else {
    const lat = Number(form.latitude);
    if (isNaN(lat) || lat < -90 || lat > 90) {
      errors.latitude = "Must be between -90 and 90.";
    }
  }

  if (!form.longitude.trim()) {
    errors.longitude = "Longitude is required.";
  } else {
    const lng = Number(form.longitude);
    if (isNaN(lng) || lng < -180 || lng > 180) {
      errors.longitude = "Must be between -180 and 180.";
    }
  }

  return { errors, isValid: Object.keys(errors).length === 0 };
}
