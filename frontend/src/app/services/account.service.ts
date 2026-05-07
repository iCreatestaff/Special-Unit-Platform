import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { catchError, map, Observable, of, throwError } from 'rxjs';
import { Account } from '../Models/account.model';
import { Mission } from '../Models/mission.model';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private apiUrl = 'http://localhost:5038/api/accounts';
  private accountMissionUrl = 'http://localhost:5038/api/AccountMission';



  constructor(private http: HttpClient) {}

  // GET all accounts
  getAccounts(): Observable<Account[]> {
    return this.http.get<Account[]>(`${this.apiUrl}/all`);
      
  }
 
  getAvailableAccounts(startDate: string, endDate: string): Observable<Account[]> {
    const params = new HttpParams()
      .set('startDate', new Date(startDate).toISOString())
      .set('endDate', new Date(endDate).toISOString());

    return this.http.get<Account[]>(`${this.apiUrl}/available`,{ params });
  }

  getAccountMissions(accountId: number): Observable<Mission[]> {
    return this.http.get<Mission[]>(
      `${this.accountMissionUrl}/assigned-missions/${accountId}`);
  }

  // GET account by ID
  getAccountById(id: number): Observable<Account> {
    return this.http.get<Account>(`${this.apiUrl}/${id}`);
  }

  // Get accounts by role
  getAccountsByRole(role: string): Observable<Account[]> {
    return this.http.get<Account[]>(`${this.apiUrl}/by-role/${role}`);
  }

  // Get accounts by type
  getAccountsByType(type: string): Observable<Account[]> {
    return this.http.get<Account[]>(`${this.apiUrl}/byType/${type}`);
  }

  // Get account by username
  getByUsername(username: string): Observable<Account> {
    return this.http.get<Account>(`${this.apiUrl}/username/${username}`);
  }

  filterAccounts(filters: { role?: string; type?: string; username?: string }): Observable<Account[]> {
    return this.getAccounts().pipe(
      map((accounts) => {
        // Apply filters locally to find the intersection
        return accounts.filter((account) => {
          // Check if the account matches all provided filters
          const matchesRole = !filters.role || account.role === filters.role;
          const matchesType = !filters.type || account.type === filters.type;
          const matchesUsername = !filters.username || account.username === filters.username;
  
          // Return true only if all filters match
          return matchesRole && matchesType && matchesUsername;
        });
      }),
      catchError((error) => {
        console.error('Error during account filtering:', error);
        return of([]); // Return an empty array on error
      })
    );
  }

  // Create a new admin account
  createAdmin(account: Account): Observable<Account> {
    return this.http.post<Account>(
      `${this.apiUrl}/create-admin`, 
      account,{ responseType: 'text' as 'json' });
  }

  // Create a new agent account
  createAgent(account: Account): Observable<Account> {
    return this.http.post<Account>(
      `${this.apiUrl}/create-agent`, 
      account,{ responseType: 'text' as 'json' });
  }

  // Update an existing account
  updateAccount(id: number, account: Account): Observable<Account> {
    return this.http.put<Account>(
      `${this.apiUrl}/update/${id}`, 
      account,{ responseType: 'text' as 'json' });
  }

  // Delete an account by ID
  deleteAccount(accountId: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${accountId}`,{ responseType: 'text' as 'json' });
  }


  // Standardized error handler
  private handleError<T>(operation = 'operation') {
    return (error: HttpErrorResponse): Observable<T> => {
      console.error(`${operation} failed:`, error);
      
      let errorMessage = 'An error occurred';
      if (error.error instanceof ErrorEvent) {
        // Client-side error
        errorMessage = error.error.message;
      } else {
        // Server-side error
        errorMessage = error.error?.message || error.message || 'Server error';
      }
      
      return throwError(() => new Error(errorMessage));
    };
  }
}