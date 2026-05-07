// unauthorized.component.ts
import { Component } from '@angular/core';

@Component({
  selector: 'app-unauthorized',
  template: `
    <div class="container">
      <h1>403 - Access Denied</h1>
      <p>You don't have permission to access this page.</p>
      <button mat-button color="primary" routerLink="/dashboard">Go to Dashboard</button>
    </div>
  `,
  styles: [`
    .container {
      text-align: center;
      padding: 50px;
    }
  `]
})
export class UnauthorizedComponent {}