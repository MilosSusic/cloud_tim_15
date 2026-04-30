export const SprayStatus ={
  Scheduled: 1,
  Cancelled: 2,
  Completed: 3,
  Rescheduled: 4,
} as const;

export type SprayStatus = typeof SprayStatus[keyof typeof SprayStatus];