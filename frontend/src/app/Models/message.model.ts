export interface Message {
    id?: number;
    senderId: number;
    receiverId: number;
    content: string;
    isRead?: boolean;
    timestamp?: string | Date;
  }
