import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { Mission } from 'src/app/Models/mission.model';
import { MissionService } from 'src/app/services/mission.service';
import { MissionFormComponent } from '../mission modal/mission-form.component';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../delete mission/confirm-dialog.component';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatMenuModule } from '@angular/material/menu';
import { CommonModule } from '@angular/common';
import { MissionDetailsComponent } from '../mission details/mission-details.component';
import { MatButtonModule } from '@angular/material/button';
import {  MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { PaginatorModule } from 'primeng/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MaterialModule } from 'src/app/material.module';
import { Observable } from 'rxjs';
@Component({
    selector: 'app-mission',
    standalone:true,
    imports: [
        MatTableModule,
        MatIconModule,
        MatCardModule,
        MatMenuModule,
        CommonModule,
        MatButtonModule,
        PaginatorModule,
        MatProgressSpinnerModule ,
        MatFormFieldModule,MaterialModule
        
    ],
    templateUrl: './mission.component.html',
    styleUrls: ['./mission.component.css'],
    encapsulation: ViewEncapsulation.Emulated,
})
export class MissionComponent implements OnInit, OnDestroy {
  missions: Mission[] = [];
  displayedColumns: string[] = ['description', 'location', 'status', 'actions'];
  dataSource = new MatTableDataSource<Mission>();
  isLoading = false;
  
  // Pagination properties
  first = 0;
  rows = 10;
  totalRecords = 0;
  cityNames: { [missionId: number]: string } = {};
  private isDestroyed = false;



  constructor(
    private missionService: MissionService,
    public dialog: MatDialog,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef
  ) {}

  
  ngOnInit(): void {
    this.loadMissions();
  }  

  ngOnDestroy(): void {
    this.isDestroyed = true;
  }

  getCityFromCoordinates(latLng: string): Promise<string> {
    const [lat, lng] = latLng.split(',').map(coord => parseFloat(coord));
    if (!Number.isFinite(lat) || !Number.isFinite(lng)) {
      return Promise.resolve('Unknown');
    }

    const url = `https://nominatim.openstreetmap.org/reverse?lat=${lat}&lon=${lng}&format=json`;
  
    return fetch(url, {
      headers: {
        'Accept': 'application/json',
        'Accept-Language': 'en'
      }
    })
      .then(res => res.json())
      .then(data => data.address?.city || data.address?.town || data.address?.village || 'Unknown')
      .catch(() => 'Unknown');
  }
  
  loadMissions(): void {
    this.isLoading = true;
    this.cityNames = {};
    this.refreshView();
  
    const accountId = Number(localStorage.getItem('accountId'));
    const role = localStorage.getItem('role');
  
    if (!accountId || !role) {
      this.showError('User information not found');
      this.isLoading = false;
      this.refreshView();
      return;
    }
  
    let missionObservable: Observable<Mission[]>;
  
    switch (role.toLowerCase()) {
      case 'agent':
        missionObservable = this.missionService.getMissionByAgentId(accountId);
        break;
      case 'admin':
        missionObservable = this.missionService.getMissionByAdminId(accountId);
        break;
      case 'superadmin':
        missionObservable = this.missionService.getMissions();
        break;
      default:
        this.showError('Invalid user role');
        this.isLoading = false;
        this.refreshView();
        return;
    }
  
    missionObservable.subscribe({
      next: (missions: Mission[]) => {
        this.missions = missions || [];
        this.totalRecords = this.missions.length;
        this.first = 0;
        this.updatePaginatedData();
        this.isLoading = false;
        this.refreshView();
        this.loadCityNames(this.missions);
      },
      error: (err: any) => {
        if (err?.status === 404) {
          this.missions = [];
          this.totalRecords = 0;
          this.updatePaginatedData();
          this.isLoading = false;
          this.refreshView();
          return;
        }

        this.showError('Failed to load missions');
        console.error('Error loading missions:', err);
        this.isLoading = false;
        this.refreshView();
      }
    });
  }
  
  onPageChange(event: any): void {
    this.first = event.first;
    this.rows = event.rows;
    this.updatePaginatedData();
  }

  private updatePaginatedData(): void {
    const startIndex = this.first;
    const endIndex = startIndex + this.rows;
    this.dataSource.data = this.missions.slice(startIndex, endIndex);
    this.refreshView();
  }

  private async loadCityNames(missions: Mission[]): Promise<void> {
    await Promise.all(
      missions.map(async (mission) => {
        if (mission.id === undefined || !mission.location) {
          return;
        }

        const city = await this.getCityFromCoordinates(mission.location);
        if (this.isDestroyed) {
          return;
        }

        this.cityNames = {
          ...this.cityNames,
          [mission.id]: city
        };
        this.refreshView();
      })
    );
  }

  formatLocation(location: string | undefined): string {
    if (!location) {
      return 'Unknown';
    }

    const [lat, lng] = location.split(',').map(coord => parseFloat(coord));
    if (!Number.isFinite(lat) || !Number.isFinite(lng)) {
      return location;
    }

    return `${lat.toFixed(4)}, ${lng.toFixed(4)}`;
  }



  openMissionDetails(mission: Mission): void {
    this.dialog.open(MissionDetailsComponent, {
      width: '100%',
      maxWidth: '1300px',
      height: '100%',
      maxHeight: '90vh',
      data: { id: mission.id },
      panelClass: 'custom-dialog-container',
    });
  }

  openMissionDialog(mission: Mission | null = null): void {
    const dialogRef = this.dialog.open(MissionFormComponent, {
      width: '90vw',
      maxWidth: '1000px',
      height: '70vh',
      data: mission ? { id: mission.id } : null
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadMissions();
        this.showSuccess(`Mission ${mission ? 'updated' : 'created'} successfully`);
      }
    });
  }
  confirmDelete(missionId: number | undefined): void {
    if (missionId === undefined) {
      console.error('Mission ID is undefined');
      return;
    }

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: { missionId },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.missionService.deleteMission(missionId).subscribe({
          next: () => {
            this.loadMissions();
            this.showSuccess('Mission deleted successfully');
          },
          error: (err) => {
            this.showError('Failed to delete mission');
            console.error('Error deleting mission:', err);
          }
        });
      }
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
      duration: 5000,
      panelClass: ['error-snackbar']
    });
  }
  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
    this.refreshView();
  }
  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'pending': return 'status-pending';
      case 'started': return 'status-started';
      case 'accomplished': return 'status-accomplished';
      case 'cancelled': return 'status-cancelled';
      default: return 'status-default';
    }
  }

  private refreshView(): void {
    if (!this.isDestroyed) {
      this.cdr.detectChanges();
    }
  }
}
