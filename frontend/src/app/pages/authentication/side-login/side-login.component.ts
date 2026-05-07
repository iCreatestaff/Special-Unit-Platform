import {
  ChangeDetectionStrategy,
  Component,
  ViewEncapsulation,
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
  FormsModule,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { AuthService } from 'src/app/services/auth.service';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-side-login',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './side-login.component.html',
  styleUrls: ['./side-login.component.css'],

})
export class AppSideLoginComponent {
  loginForm: FormGroup;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private accountService: AccountService,
    private router: Router,
    private fb: FormBuilder
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  login(): void {
    if (this.loginForm.invalid) return;

    const { username, password } = this.loginForm.value;

    this.authService.login(username, password).subscribe({
      next: () => {
        this.accountService.getByUsername(username).subscribe({
          next: (account) => {
            localStorage.setItem('accountId', account.id.toString());
            localStorage.setItem('role',account.role);
            localStorage.setItem('type',account.type);
            console.log('Stored role:', account.role);
console.log('From localStorage:', localStorage.getItem('role'));

            this.router.navigate(['/dashboard']);
          },
          error: () => {
            this.errorMessage = 'Account not found after login.';
          },
        });
      },
      error: (err) => {
        this.errorMessage =
          err.error?.message || 'Login failed. Please try again.';
      },
    });
  }
}
