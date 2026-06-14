export const CropType ={
  None: 0,
  Wheat: 1,
  Rice: 2,
  Barley: 3,
  Oats: 4,
  Corn: 5,
  Sorghum: 6,
  Buckwheat: 7,
  Chia: 8,
  Sunflower: 9,
} as const;

export type CropType = typeof CropType[keyof typeof CropType];