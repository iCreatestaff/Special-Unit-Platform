import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Maintenance, MaintenanceGrouped } from '../Models/maintenance.model';

@Injectable({
  providedIn: 'root',
})
export class MaintenanceService {
  private baseUrl = `http://localhost:5038/api/Maintenance`; // Corrected URL format (single slash)

  constructor(private http: HttpClient) {}

  // Get all maintenances
  getAllMaintenances(): Observable<Maintenance[]> {
    return this.http.get<Maintenance[]>(this.baseUrl);
  }

  // Get a maintenance by ID
  getMaintenanceById(id: number): Observable<Maintenance> {
    return this.http.get<Maintenance>(`${this.baseUrl}/${id}`);
  }
  getAllMaintenancesGrouped(): Observable<MaintenanceGrouped[]> {
    return this.http.get<MaintenanceGrouped[]>(`${this.baseUrl}/grouped`);
  }
  editMaintenance(id: number, maintenance: Maintenance):Observable<Maintenance>{
    return this.http.put<Maintenance>(`${this.baseUrl}/${id}`, maintenance);
  }
  
}
