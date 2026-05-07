import { Item } from "./item.model";
import { SubEquipment } from "./sub-equipment.model";

export interface Maintenance {
    date: string | number | Date;
    id: number;
    name: string;
    type: string;
    status: string;
    subEquipment:SubEquipment;
    description: string;
    cycle:string;
    subEquipmentId?: number;
    maintenanceDate: string;  
    maintenanceEndDate: string;  // The date format string
    // The date format string
    items?: Item[];
  }
export interface MaintenanceGrouped{
  subEquipmentId: number;
  name:string;
  maintenances:Maintenance[];
  

}
 