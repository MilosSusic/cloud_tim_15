import type { HiveLogFormState } from "../types/HiveLogEntry";

const EMPTY_GUID = "00000000-0000-0000-0000-000000000000";

export interface HiveLogFormErrors {
  reviewDate?: string;
  reviewType?: string;
  bottomBoardColor?: string;
  honeyFrames?: string;
  honeyAmountKg?: string;
  broodFrames?: string;
  note?: string;
  form?: string;
}

const isInvalidGuid = (value: string) => !value.trim() || value === EMPTY_GUID;

const validateNonNegative = (value: string): string | undefined => {
  if (value.trim() === "") {
    return "Value is required.";
  }

  const numeric = Number(value);
  if (isNaN(numeric) || numeric < 0) {
    return "Must be greater than or equal to 0.";
  }

  return undefined;
};

export function useHiveLogValidation(
  form: HiveLogFormState,
  hiveId: string,
  entryId?: string | null
): { errors: HiveLogFormErrors; isValid: boolean } {
  const errors: HiveLogFormErrors = {};

  if (isInvalidGuid(hiveId)) {
    errors.form = "Invalid hive ID.";
  }

  if (entryId !== undefined && entryId !== null && isInvalidGuid(entryId)) {
    errors.form = "Invalid log entry ID.";
  }

  if (!form.reviewDate.trim()) {
    errors.reviewDate = "Review date is required.";
  } else {
    const reviewDate = new Date(form.reviewDate);
    if (isNaN(reviewDate.getTime())) {
      errors.reviewDate = "Review date is not valid.";
    } else {
      const maxAllowed = new Date(Date.now() + 5 * 60 * 1000);
      if (reviewDate > maxAllowed) {
        errors.reviewDate = "Review date cannot be in the future (max now + 5 minutes).";
      }
    }
  }

  if (!form.reviewType.trim()) {
    errors.reviewType = "Review type is required.";
  } else if (form.reviewType.length > 100) {
    errors.reviewType = "Max 100 characters.";
  }

  if (form.bottomBoardColor && form.bottomBoardColor.length > 50) {
    errors.bottomBoardColor = "Max 50 characters.";
  }

  const honeyFramesError = validateNonNegative(form.honeyFrames);
  if (honeyFramesError) {
    errors.honeyFrames = honeyFramesError;
  }

  const honeyAmountKgError = validateNonNegative(form.honeyAmountKg);
  if (honeyAmountKgError) {
    errors.honeyAmountKg = honeyAmountKgError;
  }

  const broodFramesError = validateNonNegative(form.broodFrames);
  if (broodFramesError) {
    errors.broodFrames = broodFramesError;
  }

  if (form.note && form.note.length > 512) {
    errors.note = "Max 512 characters.";
  }

  return {
    errors,
    isValid: Object.keys(errors).length === 0,
  };
}
