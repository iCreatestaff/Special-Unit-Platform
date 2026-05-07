import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, ChangeDetectionStrategy, Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MaterialModule } from 'src/app/material.module';
import { Account } from 'src/app/Models/account.model';
import { AccountService } from 'src/app/services/account.service';
import { MatDialog } from '@angular/material/dialog';
import { AccountModalComponent } from '../account modal/account-modal.component';
import { AccountModalDeleteComponent } from '../delete account/account-modal-delete.component';
import { AccountModalDetailsComponent } from '../account details/account-modal-details.component';
import { NonAvailabilityModalComponent } from '../nonAvailability/nonavailability-modal.component';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { debounceTime, distinctUntilChanged, Subject, takeUntil } from 'rxjs';
import { PaginatorModule } from 'primeng/paginator';
import { BrowserAnimationsModule, NoopAnimationsModule } from '@angular/platform-browser/animations';
import { animate, style, transition, trigger } from '@angular/animations';


@Component({
  selector: 'app-account',
  standalone: true,
  imports: [
    MatTableModule,
    MatIconModule,
    CommonModule,
    MaterialModule,
    MatButtonModule,
    PaginatorModule,
    FormsModule,
    MatInputModule,ReactiveFormsModule ,
  ],
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css'],
   animations: [
    trigger('fadeIn', [
      transition(':enter', [
        style({ opacity: 0, height: 0 }),
        animate('300ms ease-out', style({ opacity: 1, height: '*' }))
      ]),
      transition(':leave', [
        animate('300ms ease-in', style({ opacity: 0, height: 0 }))
      ])
    ])
  ],
  encapsulation: ViewEncapsulation.Emulated,
 
})
export class AccountComponent implements OnInit, OnDestroy {
  // Data properties
  dataSource: Account[] = [];
  paginatedDataSource: Account[] = [];
  availableRoles: string[] = ['Admin', 'Agent', 'Superadmin'];
  availableTypes: string[] = ['Admin', 'Maintenance', 'Operation'];
  
  // Pagination
  first: number = 0;
  rows: number = 12;
  totalRecords: number = 0;
  
  // UI State
  loading = false;
  showFilterSection = false;
  
  // Filters
  roleFilter = new FormControl<string[]>([]);
  typeFilter = new FormControl<string[]>([]);
  usernameFilter = new FormControl<string>('');
  
  private destroy$ = new Subject<void>();
  private isDestroyed = false;

  constructor(
    private accountService: AccountService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadAccounts();
    this.setupFilterListeners();
  }

  ngOnDestroy(): void {
    this.isDestroyed = true;
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadAccounts(): void {
    this.loading = true;
    const role = localStorage.getItem('role');
    if (role === 'SuperAdmin') {
      this.loadAllAccounts();
    } else {
      this.loadAgentsForAdmin();
    }
  }

  private loadAllAccounts(): void {
    this.accountService.getAccounts().subscribe({
      next: (accounts) => {
        this.handleLoadedAccounts(accounts);
      },
      error: (err) => {
        this.handleLoadError(err);
      }
    });
  }
private handleLoadedAccounts(accounts: Account[]): void {
    this.dataSource = accounts;
    this.totalRecords = accounts.length;
    this.updatePaginatedDataSource();
    this.extractFilterOptions(accounts);
    this.loading = false;
    this.refreshView();
  }

  private handleLoadError(err: any): void {
    this.showError('Failed to load accounts. Please try again later.');
    console.error('Failed to load accounts:', err);
    this.loading = false;
    this.refreshView();
  }
  private loadAgentsForAdmin(): void {
    this.accountService.getAccountsByRole('Agent').subscribe({
      next: (accounts) => {
        this.handleLoadedAccounts(accounts);
      },
      error: (err) => {
        this.handleLoadError(err);
      }
    });
  }
  private extractFilterOptions(accounts: Account[]): void {
    // Extract unique roles and types from accounts if not predefined
    const roles = new Set(accounts.map(a => a.role));
    const types = new Set(accounts.map(a => a.type));
    
    this.availableRoles = Array.from(roles);
    this.availableTypes = Array.from(types);
  }

 private setupFilterListeners(): void {
  this.usernameFilter.valueChanges
    .pipe(debounceTime(400), distinctUntilChanged(), takeUntil(this.destroy$))
    .subscribe(() => this.applyFilters());

  this.roleFilter.valueChanges
    .pipe(takeUntil(this.destroy$))
    .subscribe(() => this.applyFilters());

  this.typeFilter.valueChanges
    .pipe(takeUntil(this.destroy$))
    .subscribe(() => this.applyFilters());
}

  updatePaginatedDataSource(): void {
    const start = this.first;
    const end = start + this.rows;
    this.paginatedDataSource = this.dataSource.slice(start, end);
    this.refreshView();
  }

  onPageChange(event: any): void {
    this.first = event.first;
    this.rows = event.rows;
    this.updatePaginatedDataSource();
  }

 applyFilters(): void {
  this.loading = true;
  this.first = 0; // Reset pagination

  const roleFilter = this.roleFilter.value || [];
  const typeFilter = this.typeFilter.value || [];
  const usernameFilter = this.usernameFilter.value?.toLowerCase() || '';

  // Filter in-memory list instead of refetching
  const filteredAccounts = this.dataSource.filter(account => {
    const matchesRole = roleFilter.length === 0 || roleFilter.includes(account.role);
    const matchesType = typeFilter.length === 0 || typeFilter.includes(account.type);
    const matchesUsername = !usernameFilter || account.username.toLowerCase().includes(usernameFilter);
    return matchesRole && matchesType && matchesUsername;
  });

  this.totalRecords = filteredAccounts.length;
  this.paginatedDataSource = filteredAccounts.slice(this.first, this.first + this.rows);
  this.loading = false;
  this.refreshView();
}


  toggleFilterSection(): void {
    this.showFilterSection = !this.showFilterSection;
  }

  clearFilters(): void {
    this.roleFilter.setValue([]);
    this.typeFilter.setValue([]);
    this.usernameFilter.setValue('');
    this.applyFilters();
  }

  openCreateAccountModal(): void {
    const dialogRef = this.dialog.open(AccountModalComponent, {
      width: '92vw',
      maxWidth: '960px',
      maxHeight:'90vh',
      panelClass: 'modern-dialog',
      data: null
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadAccounts();
    });
  }

  openUpdateAccountModal(accountId: number): void {
    const dialogRef = this.dialog.open(AccountModalComponent, {
      width: '92vw',
      maxWidth: '960px',
      maxHeight:'90vh',
      panelClass: 'modern-dialog',
      data: { accountId }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadAccounts();
    });
  }

  viewAccountDetails(accountId: number): void {
    this.dialog.open(AccountModalDetailsComponent, {
      width: '100%',
      maxWidth: '1200px',
      height:'100%',
      maxHeight:'80vh',
      data: { accountId }
    });
  }

  deleteAccount(accountId: number): void {
    const dialogRef = this.dialog.open(AccountModalDeleteComponent, {
      width: '500px',
      panelClass: 'modern-dialog',
      data: { accountId }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.accountService.deleteAccount(accountId).subscribe({
          next: () => {
            this.loadAccounts();
            this.showSuccess('Account deleted successfully');
          },
          error: (err) => {
            this.showError('Failed to delete account');
            console.error('Delete error:', err);
          }
        });
      }
    });
  }

  openNonAvailabilityModal(accountId: number): void {
    const dialogRef = this.dialog.open(NonAvailabilityModalComponent, {
      width: '600px',
      panelClass: 'modern-dialog',
      data: { accountId }
    });
  
    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadAccounts();
    });
  }

  private showSuccess(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      panelClass: ['success-snackbar']
    });
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      panelClass: ['error-snackbar']
    });
  }

  private refreshView(): void {
    if (!this.isDestroyed) {
      this.cdr.detectChanges();
    }
  }
}
