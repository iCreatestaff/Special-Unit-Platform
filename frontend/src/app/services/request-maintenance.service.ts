import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RequestMaintenance } from '../Models/request-maintenance.model';
interface EquipmentResponse {
  equipmentId: number;
  equipmentName: string;
}

interface RequestsResponse {
  equipmentName: string;
  requests: any[];
  message: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class RequestMaintenanceService {
  private apiUrl = 'http://localhost:5038/api/requestmaintenance'; // Replace with your API URL

  constructor(private http: HttpClient) {}

  createRequestMaintenance(request: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, request);
  }

  getRequestMaintenanceById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  getAllRequestMaintenances(): Observable<EquipmentResponse[]> {
    return this.http.get<EquipmentResponse[]>(this.apiUrl);
  }

  updateRequestMaintenanceStatus(id: number, status: string): Observable<any> {
    return this.http.put(
      `${this.apiUrl}/${id}/status`,
      `"${status}"`,
      { headers: { 'Content-Type': 'application/json' } }
    );
  }

  getRequestMaintenancesByEquipmentId(equipmentId: number): Observable<RequestsResponse> {
    return this.http.get<RequestsResponse>(`${this.apiUrl}/equipment/${equipmentId}`);
  }
}
