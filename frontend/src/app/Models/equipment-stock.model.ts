import { Equipment } from './equipment.model';

export interface EquipmentStock {
  id?: number;
  equipmentName: string;
  quantity?: number;
  photo?:string;
  equipments?: Equipment[];
}
export interface EquipmentWithQuantity{
    equipment: Equipment;
    quantity: number;
    
}