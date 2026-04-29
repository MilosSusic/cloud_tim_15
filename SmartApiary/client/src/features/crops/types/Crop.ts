import { CropType } from "../types/CropType"; 

export interface Crop {
  id: string;
  fieldId: string;
  cropName: CropType;
  description: string;
  color: string;
  note: string | null;
}

