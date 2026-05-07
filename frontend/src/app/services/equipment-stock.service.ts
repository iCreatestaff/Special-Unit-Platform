import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, switchMap } from 'rxjs';
import { EquipmentStock } from '../Models/equipment-stock.model';

@Injectable({
  providedIn: 'root',
})
export class EquipmentStockService {
  private apiUrl = 'http://localhost:5038/api/EquipmentStock';


  constructor(private http: HttpClient) {}

  // Fetch and update internal subject
  fetchStock(): Observable<EquipmentStock[]> {
    return this.http.get<EquipmentStock[]>(this.apiUrl)
  }

  // Get by ID (always from API, no cache)
  getStockById(id: number): Observable<EquipmentStock> {
    return this.http.get<EquipmentStock>(`${this.apiUrl}/${id}`);
  }

  addStock(stock: EquipmentStock): Observable<EquipmentStock[]> {
    return this.http.post<EquipmentStock>(this.apiUrl, stock).pipe(
      switchMap(() => this.fetchStock())
    );
  }

  updateStock(id: number, stock: EquipmentStock): Observable<EquipmentStock[]> {
    return this.http.put<EquipmentStock>(`${this.apiUrl}/${id}`, stock).pipe(
      switchMap(() => this.fetchStock())
    );
  }

  updateAllEquipmentStock(equipmentStockId: number, payload: any): Observable<EquipmentStock[]> {
    return this.http.put(`${this.apiUrl}/updateAllEquipment/${equipmentStockId}`, payload).pipe(
      switchMap(() => this.fetchStock())
    );
  }

  deleteStock(id: number): Observable<EquipmentStock[]> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      switchMap(() => this.fetchStock())
    );
  }
   deleteSubEquipmentFromAllEquipments(equipmentStockId: number, subEquipmentName: string): Observable<{success: boolean, message?: string}> {
  return this.http.delete<{success: boolean, message?: string}>(
    `${this.apiUrl}/subequipment`, 
    {
      params: {
        equipmentStockId: equipmentStockId.toString(),
        subEquipmentName: subEquipmentName
      }
    }
  );
}
}
