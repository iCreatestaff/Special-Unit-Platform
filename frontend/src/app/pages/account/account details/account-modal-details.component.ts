import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, HostListener, Inject, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MaterialModule } from 'src/app/material.module';
import { Account } from 'src/app/Models/account.model';
import { Mission } from 'src/app/Models/mission.model';
import { AccountService } from 'src/app/services/account.service';
import { MissionDetailsComponent } from '../../Mission/mission details/mission-details.component';
import { MissionService } from 'src/app/services/mission.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NonAvailabilityModalComponent } from '../nonAvailability/nonavailability-modal.component';
import { AccountModalComponent } from '../account modal/account-modal.component';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-account-modal-details',
  standalone: true,
  imports: [MatTableModule, MatIconModule, CommonModule, MaterialModule],
  templateUrl: './account-modal-details.component.html',
  styleUrls: ['./account-modal-details.component.css'],
  encapsulation: ViewEncapsulation.None

})
export class AccountModalDetailsComponent implements OnInit, OnDestroy {
  account: Account | null = null; // Account details including photo and badge
  missions: Mission[] = []; // Missions associated with the account
  loading: boolean = false; // Loading state
  error: string = ''; // Error message
  private readonly destroy$ = new Subject<void>();
  private isDestroyed = false;

  constructor(
    public dialogRef: MatDialogRef<AccountModalDetailsComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { accountId: number },
    private accountService: AccountService,
    private missionService: MissionService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef
  ) {}

 // In AccountModalDetailsComponent
 ngOnInit(): void {
  if (this.data.accountId) {
    this.loadAccountDetails();
  } else {
    this.error = 'Account ID is missing';
    console.error('Account ID is undefined');
    this.refreshView();
  }
}

ngOnDestroy(): void {
  this.isDestroyed = true;
  this.destroy$.next();
  this.destroy$.complete();
}

@HostListener('document:keydown.escape')
onEscapeKey() {
  this.onClose();
}

loadAccountDetails(): void {
  this.loading = true;
  this.error = '';
  this.missions = [];
  this.refreshView();

  this.accountService.getAccountById(this.data.accountId)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
    next: (account) => {
      this.account = account;
      this.refreshView();
      this.loadMissions();
    },
    error: (err) => {
      this.loading = false;
      this.error = 'Failed to load account details';
      this.showError('Failed to load account details. Please try again.');
      console.error('Error loading account:', err);
      this.refreshView();
    }
  });
}

loadMissions(): void {
  if (this.account?.role === 'Agent') {
    this.accountService.getAccountMissions(this.data.accountId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
      next: (missions) => {
        this.missions = missions;
        this.loading = false;
        this.refreshView();
      },
      error: (err) => {
        this.loading = false;
        this.missions = [];
        if (err.status !== 404) { // 404 means no missions found
          this.showError('Failed to load missions');
          console.error('Error loading missions:', err);
        }
        this.refreshView();
      }
    });
  } else {
    this.loading = false;
    this.refreshView();
  }
}

openUpdateAccountModal(): void {
  if (!this.account) return;
  
  const dialogRef = this.dialog.open(AccountModalComponent, {
    width: '92vw',
    maxWidth: '960px',
    maxHeight: '90vh',
    panelClass: 'modern-dialog',
    data: { accountId: this.account.id }
  });

  dialogRef.afterClosed().subscribe(updated => {
    if (updated) {
      this.loadAccountDetails();
      this.showSuccess('Account updated successfully');
    }
  });
}

openNonAvailabilityModal(): void {
  if (!this.account) return;
  
  const dialogRef = this.dialog.open(NonAvailabilityModalComponent, {
    width: '600px',
    data: { accountId: this.account.id }
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result) {
      this.showSuccess('Availability updated successfully');
      this.refreshView();
    }
  });
}

assignNewMission(): void {
  if (!this.account) return;
  
  // Implement mission assignment logic
  this.showSuccess('New mission assigned');
}

openMissionDetails(mission: Mission): void {
  this.dialog.open(MissionDetailsComponent, {
    width: '100%',
    maxWidth: '1300px',
    height: '100%',
    maxHeight: '90vh',
    data: { id: mission.id }
  });
}

onClose(): void {
  this.dialogRef.close();
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
