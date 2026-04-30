export interface NearbyFieldOwner {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
}

export interface NearbyFieldCrop {
  name: string;
  icon: string;
  floweringPeriod: string;
}

export interface NearbyField {
  fieldId: string;
  fieldName: string;
  latitude: number;
  longitude: number;
  area: number;
  owner: NearbyFieldOwner;
  crops: NearbyFieldCrop[];
}
