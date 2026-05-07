// notification.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { CreateNotificationDto, Notification } from '../Models/notification.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private apiUrl = 'http://localhost:5038/api/notifications';

  constructor(private http: HttpClient) { }

  getAllNotifications(): Observable<Notification[]> {
    return this.http.get<Notification[]>(this.apiUrl).pipe(
      catchError(this.handleError)
    );
  }

  getNotificationsByRecipient(recipientId: number): Observable<Notification[]> {
    return this.http.get<Notification[]>(`${this.apiUrl}/recipient/${recipientId}`).pipe(
      catchError(this.handleError)
    );
  }

  createNotification(notification: CreateNotificationDto): Observable<Notification> {
    return this.http.post<Notification>(this.apiUrl, notification).pipe(
      catchError(this.handleError)
    );
  }

  markAsRead(notificationId: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/read/${notificationId}`, null).pipe(
      catchError(this.handleError)
    );
  }

  deleteNotification(notificationId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${notificationId}`).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: any) {
    console.error('An error occurred:', error);
    return throwError(() => new Error('Something went wrong; please try again later.'));
  }
  getByTypeAsync(type: string): Observable<Notification[]> {
    return this.http.get<Notification[]>(`${this.apiUrl}/type/${type}`);
  }
}