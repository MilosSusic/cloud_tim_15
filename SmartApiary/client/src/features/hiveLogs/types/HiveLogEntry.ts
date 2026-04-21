export interface HiveLogEntryDto {
  id: string;
  hiveId: string;
  reviewDate: string;
  reviewType: string;
  bottomBoardColor: string;
  honeyFrames: number;
  honeyAmountKg: number;
  broodFrames: number;
  queenPresent: boolean;
  note: string | null;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CreateHiveLogEntryRequest {
  hiveId: string;
  reviewDate: string;
  reviewType: string;
  bottomBoardColor: string;
  honeyFrames: number;
  honeyAmountKg: number;
  broodFrames: number;
  queenPresent: boolean;
  note?: string | null;
}

export interface UpdateHiveLogEntryRequest {
  id: string;
  reviewDate: string;
  reviewType: string;
  bottomBoardColor: string;
  honeyFrames: number;
  honeyAmountKg: number;
  broodFrames: number;
  queenPresent: boolean;
  note?: string | null;
}

export interface HiveLogFormState {
  reviewDate: string;
  reviewType: string;
  bottomBoardColor: string;
  honeyFrames: string;
  honeyAmountKg: string;
  broodFrames: string;
  queenPresent: boolean;
  note: string;
}
