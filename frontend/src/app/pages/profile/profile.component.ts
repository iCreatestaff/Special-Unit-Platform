import { CommonModule, DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

import { Router } from '@angular/router';
import { MaterialModule } from 'src/app/material.module';
import { Account } from 'src/app/Models/account.model';
import { Mission } from 'src/app/Models/mission.model';
import { AccountService } from 'src/app/services/account.service';
import { MatDialog } from '@angular/material/dialog';
import { AccountModalComponent } from '../account/account modal/account-modal.component';
import { MissionDetailsComponent } from '../Mission/mission details/mission-details.component';
import { MissionService } from 'src/app/services/mission.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MaterialModule,

    MatTabsModule,
    MatCardModule,

    MatButtonModule,

    MatTooltipModule
  ],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
  providers: [DatePipe]
})
export class ProfileComponent implements OnInit, OnDestroy {
  account: Account | null = null;
  missions: Mission[] = [];
  isLoading = true;
  errorMessage = '';
  activeTab = 0; // 'profile' or 'missions'
  private readonly destroy$ = new Subject<void>();
  private isDestroyed = false;

  constructor(
    private accountService: AccountService,
    private missionService: MissionService,
    private router: Router,
    private datePipe: DatePipe,
    private dialog: MatDialog,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadAccountData();
  }

  ngOnDestroy(): void {
    this.isDestroyed = true;
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadAccountData(): void {
    const accountId = localStorage.getItem('accountId');
    
    if (!accountId) {
      this.errorMessage = 'No user logged in';
      this.isLoading = false;
      this.refreshView();
      this.router.navigate(['/authentication/login']);
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.account = null;
    this.missions = [];
    this.refreshView();

    this.accountService.getAccountById(+accountId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
      next: (account) => {
        this.account = account;
        this.refreshView();
        this.loadMissions(account);
      },
      error: (err) => {
        this.errorMessage = 'Failed to load profile data';
        this.isLoading = false;
        console.error(err);
        this.refreshView();
      }
    });
  }

loadMissions(account: Account): void {
  if (account.role === 'Agent') {
    this.getMissionsByAgent(account.id);
  } else if (account.role === 'Admin' || account.role === 'SuperAdmin') {
    this.getMissionsByAdmin(account.id);
  } else {
    console.warn('Unknown role:', account.role);
    this.missions = [];
    this.finishLoading();
  }
}

private getMissionsByAgent(accountId: number): void {
  this.missionService.getMissionByAgentId(accountId)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
    next: (missions) => {
      this.missions = missions ?? [];
      this.finishLoading();
    },
    error: (err) => {
      this.handleMissionLoadError('agent', err);
    }
  });
}

private getMissionsByAdmin(accountId: number): void {
  this.missionService.getMissionByAdminId(accountId)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
    next: (missions) => {
      this.missions = missions ?? [];
      this.finishLoading();
    },
    error: (err) => {
      this.handleMissionLoadError('admin', err);
    }
  });
}

    openUpdateAccountModal(accountId: number): void {
      const dialogRef = this.dialog.open<AccountModalComponent>(AccountModalComponent, {
        width: '100%',
        maxWidth: '1200px',
        height:'90%',
        maxHeight:'80vh',
        panelClass: 'modern-dialog',
        data: { accountId }
      });
  
      dialogRef.afterClosed().subscribe(result => {
        if (result) this.loadAccountData();
        else this.refreshView();
      });
    }
  formatDate(date: string | Date): string {
    return this.datePipe.transform(date, 'medium') || '';
  }
   openMissionDetails(mission: Mission): void {
      this.dialog.open(MissionDetailsComponent, {
        width: '100%',
        maxWidth: '1300px',
        height: '100%',
        maxHeight: '90vh',
        data: { id: mission.id },
       
      });
    }
  
  getStatusColor(status: string): string {
    const statusMap: any = {
      started: 'started',
      pending: 'pending',
      accomplished: 'accomplished'
    };
    return statusMap[status.toLowerCase()] || 'unknown';
  }

  private handleMissionLoadError(scope: 'agent' | 'admin', err: any): void {
    if (err?.status !== 404) {
      console.error(`Failed to load ${scope} missions`, err);
    }

    this.missions = [];
    this.finishLoading();
  }

  private finishLoading(): void {
    this.isLoading = false;
    this.refreshView();
  }

  private refreshView(): void {
    if (!this.isDestroyed) {
      this.cdr.detectChanges();
    }
  }
  
}
