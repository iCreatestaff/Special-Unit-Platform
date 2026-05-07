import { SubEquipment } from "./sub-equipment.model";

export interface Equipment {
  id: number;
  name?: string;
  availability?: boolean;
  type?: string;
  subEquipments?: SubEquipment[];
  photo?:string;
  equipmentStockId?:number;
}