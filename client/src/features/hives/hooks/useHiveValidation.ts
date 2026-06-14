export interface HiveFormErrors {
  label?: string;
  type?: string;
  color?: string;
  queenAge?: string;
  note?: string;
}

interface HiveFormFields {
  label: string;
  type: string;
  color: string;
  queenAge: string;
  note: string;
}

export function useHiveValidation(form: HiveFormFields): {
  errors: HiveFormErrors;
  isValid: boolean;
} {
  const errors: HiveFormErrors = {};

  if (!form.label.trim()) {
    errors.label = "Label is required.";
  } else if (form.label.length > 50) {
    errors.label = "Max 50 characters.";
  }

  if (!form.type.trim()) {
    errors.type = "Type is required.";
  } else if (form.type.length > 50) {
    errors.type = "Max 50 characters.";
  }

  if (!form.color.trim()) {
    errors.color = "Color is required.";
  } else if (form.color.length > 50) {
    errors.color = "Max 50 characters.";
  }

  if (form.queenAge === "" || isNaN(Number(form.queenAge))) {
    errors.queenAge = "Queen age is required.";
  } else {
    const age = Number(form.queenAge);
    if (age < 0 || age > 10) {
      errors.queenAge = "Must be between 0 and 10.";
    }
  }

  if (form.note && form.note.length > 512) {
    errors.note = "Max 512 characters.";
  }

  return { errors, isValid: Object.keys(errors).length === 0 };
}
