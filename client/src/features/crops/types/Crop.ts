import { CropType } from "../types/CropType"; 

export interface Crop {
  id: string;
  fieldId: string;
  cropName: CropType;
  description: string;
  floweringPeriodStart: string;
  floweringPeriodEnd: string;
  isDeleted: boolean;
}

