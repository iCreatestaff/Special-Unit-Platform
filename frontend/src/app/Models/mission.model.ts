export interface Mission {
  id?: number;
  type?: string;
  description: string;
  startTime: string;
  endTime: string;
  location: string;
  status?: string;
  adminId: number;
  assignedAccounts: number[];
  assignedEquipments: number[];
}
