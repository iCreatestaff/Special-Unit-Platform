import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly baseUrl = 'http://localhost:5038/api/Auth';
  private readonly tokenKey = 'jwt_token';
  private readonly roleKey = 'role';

  constructor(private http: HttpClient) {}

  login(username: string, password: string) {
    return this.http.post<{ token: string, role: string }>(`${this.baseUrl}/login`, { username, password }).pipe(
      tap(response => {
        localStorage.setItem(this.tokenKey, response.token);
        localStorage.setItem(this.roleKey, response.role);
      })
    );
  }

  getRole(): string | null {
    return localStorage.getItem(this.roleKey);
  }

  isSuperAdmin(): boolean {
    return this.getRole() === 'SuperAdmin';
  }

  isAgent(): boolean {
    return this.getRole() === 'Agent';
  }

  isAdmin(): boolean {
    return this.getRole() === 'Admin';
  }

  isTechnician(): boolean {
    return this.getRole() === 'Technician';
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.roleKey);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}