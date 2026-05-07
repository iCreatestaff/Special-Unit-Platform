import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Equipment } from '../Models/equipment.model';
import { EquipmentWithQuantity } from '../Models/equipment-stock.model';

@Injectable({
  providedIn: 'root',
})
export class EquipmentService {
  private apiUrl = 'http://localhost:5038/api/equipment'; 

  constructor(private http: HttpClient) {}

  getEquipments(): Observable<Equipment[]> {
    return this.http.get<Equipment[]>(this.apiUrl);
  }
  getAvailableEquipments(startDate: string, endDate: string): Observable<Equipment[]> {
    const start = new Date(startDate).toISOString(); // Convert to ISO string
    const end = new Date(endDate).toISOString(); // Convert to ISO string
    const params = new HttpParams()
         .set('startDate', start)
         .set('endDate', end);
  
    return this.http.get<Equipment[]>(`${this.apiUrl}/available`,{ params });
  }
   
  getEquipmentById(id: number): Observable<Equipment> {
    return this.http.get<Equipment>(`${this.apiUrl}/${id}`);
  }

  createEquipment(equipment: Equipment): Observable<Equipment> {
    return this.http.post<Equipment>(this.apiUrl, equipment);
  }

  createEquipmentWithQuantity(equipmentWithQuantity: EquipmentWithQuantity): Observable<Equipment> {
    return this.http.post<Equipment>(`${this.apiUrl}/create-with-quantity`, equipmentWithQuantity,{ responseType: 'text' as 'json' });
  }

  updateEquipment(id: number, equipment: Equipment): Observable<Equipment> {
    return this.http.put<Equipment>(`${this.apiUrl}/${id}`, equipment);
  }

  deleteEquipment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
