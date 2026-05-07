// notification.model.ts
export interface Notification {
  id: number;
  type: string;
  isRead: boolean;
  details: string;
  recipientId?: number;
  referenceId?: number;
  createdAt: Date;
}

export interface CreateNotificationDto {
  type: string;
  details: string;
  recipientId?: number;
  referenceId?: number;
}