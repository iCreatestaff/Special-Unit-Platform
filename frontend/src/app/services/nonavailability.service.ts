import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { NonAvailability } from '../Models/nonavailability.model';

@Injectable({
  providedIn: 'root',
})
export class NonAvailabilityService {
  private apiUrl = 'http://localhost:5038/api/nonavailability';

  constructor(private http: HttpClient) {}

  createNonAvailability(data: NonAvailability): Observable<any> {

    // ✅ Ensure date format is correct
    const formattedDate1 = new Date(data.date1).toISOString();
    const formattedDate2 = new Date(data.date2).toISOString();

    const requestBody = {
         // ✅ Wrapped inside "nonAvailabilityDTO"
            accountId: data.accountId,
            date1: formattedDate1,
            date2: formattedDate2
        
      
    };

    // ✅ Fix: Use requestBody with text response type
    return this.http.post(`${this.apiUrl}`, requestBody, { responseType: 'text' as 'json' });
  }

  getNonAvailabilityByAccount(accountId: number): Observable<NonAvailability[]> {
    return this.http.get<NonAvailability[]>(`${this.apiUrl}/account/${accountId}`);
  }

  updateNonAvailability(id: number, data: NonAvailability): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, data);
  }

  deleteNonAvailability(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
