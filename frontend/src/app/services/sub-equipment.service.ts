import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SubEquipment } from '../Models/sub-equipment.model';

@Injectable({
  providedIn: 'root',
})
export class SubEquipmentService {
  private apiUrl = 'http://localhost:5038/api/SubEquipment';  // Replace with your actual API endpoint

  constructor(private http: HttpClient) {}

  // GET all sub-equipments
  getAllSubEquipments(): Observable<SubEquipment[]> {
    return this.http.get<SubEquipment[]>(this.apiUrl);
  }

  // GET sub-equipment by ID
  getSubEquipmentById(id: number): Observable<SubEquipment> {
    return this.http.get<SubEquipment>(`${this.apiUrl}/${id}`);
  }

  // POST (create) a new sub-equipment
  createSubEquipment(subEquipment: SubEquipment): Observable<SubEquipment> {
    return this.http.post<SubEquipment>(this.apiUrl, subEquipment);
  }

  // PUT (update) an existing sub-equipment
  updateSubEquipment(id: number, subEquipment: SubEquipment): Observable<SubEquipment> {
    return this.http.put<SubEquipment>(`${this.apiUrl}/${id}`, subEquipment);
  }

  // DELETE a sub-equipment by ID
  deleteSubEquipment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}