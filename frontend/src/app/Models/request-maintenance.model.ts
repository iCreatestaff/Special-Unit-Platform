export interface RequestMaintenance {
  id: number;
  maintenanceId: number;
  status: 'Pending' | 'Accepted' | 'Rejected';
  details?: string;
  cycle?: string;
  maintenanceDate: string;
  equipmentId?: number;
  loading?: boolean;
  subequipmentName?: string;
  equipmentInUseInMission?: boolean;
}

export interface EquipmentGroup {
  equipmentId: number;
  equipmentName: string;
  requests: RequestMaintenance[];
  message?: string;
}