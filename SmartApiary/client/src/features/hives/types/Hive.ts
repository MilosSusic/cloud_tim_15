export interface Hive {
  id: string;
  apiaryId: string;
  label: string;
  type: string;
  color: string;
  queenAge: number;
  note: string | null;
}

export interface HiveFormState {
  label: string;
  type: string;
  color: string;
  queenAge: string;
  note: string;
}
