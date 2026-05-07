import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { Training } from '../Models/training.model';


@Injectable({
  providedIn: 'root'
})
export class TrainingService {
  private apiUrl = 'http://localhost:5038/api/Training';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Training[]> {
    return this.http.get<Training[]>(this.apiUrl);
  }

  getById(id: number): Observable<Training> {
    return this.http.get<Training>(`${this.apiUrl}/${id}`);
  }

  create(training: Training): Observable<any> {
    return this.http.post(this.apiUrl, training,{ responseType: 'text' as 'json' });
  }

 // In TrainingService
update(id: number, training: Training): Observable<any> {
  return this.http.put(`${this.apiUrl}/${id}`, training).pipe(
    catchError(error => {
      console.error('Error updating training:', error);
      return throwError(() => new Error('Failed to update training'));
    })
  );
}

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  assignAccount(trainingId: number, accountId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${trainingId}/assign/${accountId}`, {});
  }

  getByAgent(agentId: number): Observable<Training[]> {
    return this.http.get<Training[]>(`${this.apiUrl}/agent/${agentId}`);
  }
  deleteNonAvailability(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}/nonavailabilities`, { responseType: 'text' as 'json' });
  }
}
