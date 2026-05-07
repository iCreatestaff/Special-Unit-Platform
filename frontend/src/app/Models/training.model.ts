export interface Training {
    id?: number;
    title: string;
    description: string;
    startTime: Date;
    endTime: Date;
    location: string;
    status?: string;
    assignedAccounts: number[];
  }
  