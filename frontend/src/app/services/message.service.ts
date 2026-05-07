import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { Message } from '../Models/message.model';


@Injectable({ providedIn: 'root' })
export class MessageService {
  private apiUrl = 'http://localhost:5038/api/messages';

  constructor(private http: HttpClient) {}

  sendMessage(senderId: number, receiverId: number, content: string): Observable<Message> {
    if (!senderId || !receiverId) {
      return throwError(() => new Error('Both senderId and receiverId are required'));
    }

    if (!content || content.trim() === '') {
      return throwError(() => new Error('Message content cannot be empty'));
    }

    return this.http.post<Message>(`${this.apiUrl}/send`, {
      senderId,
      receiverId,
      content
    });
  }

  getMessagesForAgent(agentId: number): Observable<Message[]> {
    if (!agentId) {
      return throwError(() => new Error('agentId is required'));
    }

    return this.http.get<Message[]>(`${this.apiUrl}/agent/${agentId}`);
  }

  markMessageAsRead(messageId: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/read/${messageId}`, {});
  }

  getConversation(userId: number, otherUserId: number): Observable<Message[]> {
    if (!userId || !otherUserId) {
      return throwError(() => new Error('Both user IDs are required'));
    }

    const params = new HttpParams()
      .set('user1Id', userId)
      .set('user2Id', otherUserId);

    return this.http.get<Message[]>(`${this.apiUrl}/conversation`, { params });
  }

  markAllMessagesAsRead(agentId: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${agentId}/mark-read`, {});
  }
}
