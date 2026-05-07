import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MaintenanceService } from 'src/app/services/maintenance.service';
import { MatDialog } from '@angular/material/dialog';
import { Maintenance, MaintenanceGrouped } from 'src/app/Models/maintenance.model';

import { SubEquipment } from 'src/app/Models/sub-equipment.model';
import { SubEquipmentService } from 'src/app/services/sub-equipment.service';
import { MatPaginator } from '@angular/material/paginator';
import { EquipmentService } from 'src/app/services/equipment.service';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PaginatorModule } from 'primeng/paginator';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MaintenanceModalComponent } from '../maintenance details/maintenance-details.component';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-maintenance',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatFormFieldModule,
    MatSelectModule,
    MatOptionModule,
    PaginatorModule,
    MatButtonModule,FormsModule
  ],
  templateUrl: './maintenance.component.html',
  styleUrls: ['./maintenance.component.css']
})
export class MaintenanceComponent implements OnInit, OnDestroy {
  dataSource: MaintenanceGrouped[] = [];
  paginatedDataSource: MaintenanceGrouped[] = [];
  loading = false;
  errorMessage: string = '';
  first: number = 0;
  rows: number = 10;
  totalRecords: number = 0;
  selectedGroupName: string | null = null; // To store the selected group name
  maintenanceGroupNames: string[] = []; // Array to hold unique maintenance group names
  private readonly destroy$ = new Subject<void>();
  private isDestroyed = false;

  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(
    private maintenanceService: MaintenanceService,
    private dialog: MatDialog,
    private subequipmentService: SubEquipmentService,
    private equipmentService: EquipmentService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadMaintenances();
  }

  ngOnDestroy(): void {
    this.isDestroyed = true;
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadMaintenances(): void {
    this.loading = true;
    this.errorMessage = '';
    this.refreshView();

    this.maintenanceService.getAllMaintenancesGrouped()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
      next: (groups) => {
        this.dataSource = groups ?? [];
        this.totalRecords = this.dataSource.length;
        this.maintenanceGroupNames = [...new Set(this.dataSource.map(group => group.name))].sort(); // Extract unique group names and sort
        this.updatePaginatedDataSource();
        this.loading = false;
        this.refreshView();
      },
      error: (err) => {
        this.dataSource = [];
        this.paginatedDataSource = [];
        this.totalRecords = 0;
        this.errorMessage = err?.status === 404 ? '' : 'Failed to load maintenances';
        this.loading = false;
        console.error(err);
        this.refreshView();
      },
    });
  }

  applyFilters(): void {
    this.first = 0;
    this.updatePaginatedDataSource();
  }

  clearFilter(): void {
    this.selectedGroupName = null;
    this.applyFilters();
  }

  updatePaginatedDataSource(): void {
    let filtered = this.dataSource;

    if (this.selectedGroupName) {
      filtered = filtered.filter(group => group.name === this.selectedGroupName);
    }

    this.totalRecords = filtered.length;
    this.paginatedDataSource = filtered.slice(this.first, this.first + this.rows);
    this.refreshView();
  }

  onPageChange(event: any): void {
    this.first = event.first;
    this.rows = event.rows;
    this.updatePaginatedDataSource();
  }

  openMaintenanceModal(group: MaintenanceGrouped): void {
    this.dialog.open(MaintenanceModalComponent, {
      data: group,
      width: '80%',
      maxWidth: '1200px',
      height: 'auto',
      maxHeight: '85vh',
    });
  }

  getLatestDescription(maintenances: Maintenance[]): string {
    return [...maintenances]
      .sort((a, b) => new Date(b.maintenanceDate).getTime() - new Date(a.maintenanceDate).getTime())[0]?.description || '';
  }

  getNextMaintenanceDate(maintenances: Maintenance[]): Date | null {
    const now = new Date();
    return maintenances
      .map(m => new Date(m.maintenanceDate))
      .filter(date => date > now)
      .sort((a, b) => a.getTime() - b.getTime())[0] || null;
  }

  private refreshView(): void {
    if (!this.isDestroyed) {
      this.cdr.detectChanges();
    }
  }
}
