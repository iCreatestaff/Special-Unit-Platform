import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Mission } from '../Models/mission.model';

@Injectable({
  providedIn: 'root',
})
export class MissionService {
  private apiUrl = 'http://localhost:5038/api/missions';

  constructor(private http: HttpClient) {}

  getMissions(): Observable<Mission[]> {
    return this.http.get<Mission[]>(this.apiUrl);
  }

  getMissionById(id: number): Observable<Mission> {
    return this.http.get<Mission>(`${this.apiUrl}/${id}`);
  }
  getMissionByAdminId(adminId:number):Observable<Mission[]>{
    return this.http.get<Mission[]>(`${this.apiUrl}/admin/${adminId}`);
  }
  getMissionByAgentId(agentId:number):Observable<Mission[]>{
    return this.http.get<Mission[]>(`${this.apiUrl}/agent/${agentId}`);
  }
  createMission(mission: Mission): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/create`, mission, { responseType: 'text' as 'json' });
  }

  updateMission(id: number, mission: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, mission, { responseType: 'text' as 'json' });
  }

  deleteMission(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { responseType: 'text' as 'json' });
  }
  deleteNonavailabilities(MissionId:number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${MissionId}/nonavailabilities`, { responseType: 'text' as 'json' });
  }
}
