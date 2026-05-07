import { Equipment } from "./equipment.model";

export interface SubEquipment {
  id: number;
  name: string;
  cycle: string;
  status: string;
  equipmentId: number;
  creationDate: string;
  equipment?: Equipment;

}