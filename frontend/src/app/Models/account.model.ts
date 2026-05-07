import { NonAvailability } from "./nonavailability.model";

export interface Account {
  id: number;
  name: string;
  username: string;
  type:string;
  badge:string;
  password?:string;
  role: string;
  photo?:string;
  longitude?:string;
  latitude?:string;
  nonavailabilities?:NonAvailability[];
}

export interface GetAccount {
  id: number;
  name: string;
  username: string;
  type:string;
  password?:string;
  role: string;
  socialFile: string,
  medicalFile: string,
  careerFile: string
}
