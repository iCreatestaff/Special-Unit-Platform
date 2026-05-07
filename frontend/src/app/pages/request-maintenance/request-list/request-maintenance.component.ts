import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RequestMaintenanceService } from 'src/app/services/request-maintenance.service';
import { CommonModule } from '@angular/common';
import { MaterialModule } from 'src/app/material.module';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FormsModule } from '@angular/forms';
import { ConfirmDialogComponent } from '../request-modal/request-maintenance-edit.component';
import { MatDialog } from '@angular/material/dialog';
import { RequestMaintenance } from 'src/app/Models/request-maintenance.model';
import { PaginatorModule } from 'primeng/paginator';

@Component({
  selector: 'app-request-maintenance-list',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    MatSlideToggleModule,
    MatProgressSpinnerModule,
    FormsModule,
    PaginatorModule,
    ConfirmDialogComponent
  ],
  templateUrl: './request-maintenance-list.component.html',
  styleUrls: ['./request-maintenance-list.component.css']
})
export class RequestMaintenanceListComponent implements OnInit {
  loading = true;
  errorMessage = '';
  equipmentGroups: any[] = [];
  searchText: string = '';
  displayedColumns: string[] = ['details', 'MaintenanceDate', 'cycle', 'subEquipment', 'actions'];

  totalEquipment = 0;
  totalItems = 0;
  totalRequests = 0;

  first = 0;
  rows = 10;
  totalRecords = 0;

  constructor(
    private requestService: RequestMaintenanceService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadEquipmentWithRequestCounts();
  }

  async loadEquipmentWithRequestCounts(): Promise<void> {
    try {
      this.loading = true;
      const equipmentData: any = await this.requestService.getAllRequestMaintenances().toPromise();
      const groupedByName = this.groupEquipmentByName(equipmentData);

      this.equipmentGroups = await Promise.all(
        groupedByName.map(async group => {
          const equipmentItems = await Promise.all(
            group.equipmentIds.map(async (id: number) => {
              const count = await this.getRequestCountForEquipment(id);
              return {
                equipmentId: id,
                requestCount: count,
                requests: [],
                loadingRequests: false,
                expanded: false,
                equipmentInUseInMission: false
              };
            })
          );
          return {
            equipmentName: group.equipmentName,
            equipmentItems: equipmentItems,
            expanded: false
          };
        })
      );

      this.totalEquipment = this.equipmentGroups.length;
      this.totalItems = equipmentData.length;
      this.totalRequests = this.equipmentGroups.reduce((sum: number, group: any) => {
        return sum + group.equipmentItems.reduce((innerSum: number, item: any) => innerSum + item.requestCount, 0);
      }, 0);

      this.totalRecords = this.filteredEquipmentGroups().length;

    } catch (error) {
      console.error('Error loading equipment', error);
      this.errorMessage = 'Failed to load equipment list';
    } finally {
      this.loading = false;
    }
  }

  private async getRequestCountForEquipment(equipmentId: number): Promise<number> {
    try {
      const requestsData: any = await this.requestService.getRequestMaintenancesByEquipmentId(equipmentId).toPromise();
      return requestsData.requests?.length || 0;
    } catch (error) {
      console.error('Error getting request count for equipment', equipmentId, error);
      return 0;
    }
  }

  async loadEquipmentRequests(equipmentItem: any): Promise<void> {
    if (equipmentItem.requests?.length > 0) return;

    try {
      equipmentItem.loadingRequests = true;
      const requestsData: any = await this.requestService.getRequestMaintenancesByEquipmentId(equipmentItem.equipmentId).toPromise();
      equipmentItem.requests = (requestsData.requests || []).map((r: any) => ({
        ...r,
        loading: false,
        equipmentInUseInMission: r.equipmentInUseInMission || false
      }));

      equipmentItem.equipmentInUseInMission = equipmentItem.requests.some(
        (r: RequestMaintenance) => r.equipmentInUseInMission
      );
    } catch (error) {
      console.error('Error loading requests', error);
    } finally {
      equipmentItem.loadingRequests = false;
    }
  }

  filteredEquipmentGroups(): any[] {
    const filtered = this.equipmentGroups.filter(group =>
      group.equipmentName.toLowerCase().includes(this.searchText.toLowerCase())
    );
    this.totalRecords = filtered.length;

    // Apply pagination here
    const start = this.first;
    const end = this.first + this.rows;
    return filtered.slice(start, end);
  }

  private groupEquipmentByName(equipmentData: any[]): any[] {
    const groupsMap = new Map<string, number[]>();
    equipmentData.forEach(equipment => {
      if (!groupsMap.has(equipment.equipmentName)) {
        groupsMap.set(equipment.equipmentName, []);
      }
      groupsMap.get(equipment.equipmentName)?.push(equipment.equipmentId);
    });

    return Array.from(groupsMap.entries()).map(([name, ids]) => ({
      equipmentName: name,
      equipmentIds: ids
    }));
  }

  async confirmStatusChange(request: any, newStatus: 'Accepted' | 'Rejected'): Promise<void> {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '500px',
      data: { request, newStatus },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.confirmed) {
        this.updateRequestStatus(request, newStatus);
      }
    });
  }

  async updateRequestStatus(request: any, newStatus: string): Promise<void> {
    try {
      request.loading = true;
      await this.requestService.updateRequestMaintenanceStatus(request.id, newStatus).toPromise();
      request.status = newStatus;
      this.snackBar.open(`Request ${newStatus.toLowerCase()} successfully`, 'Close', { duration: 3000 });
    } catch (error) {
      console.error('Error updating request status', error);
      this.snackBar.open('Failed to update request status', 'Close', { duration: 3000 });
    } finally {
      request.loading = false;
    }
  }

  toggleEquipmentExpansion(equipment: any): void {
    equipment.expanded = !equipment.expanded;

    // Initialize requests array if not present
    if (!equipment.requests) {
      equipment.requests = [];
    }

    if (equipment.expanded && equipment.requests.length === 0) {
      this.loadEquipmentRequests(equipment);
    }
  }

  onPageChange(event: any): void {
    this.first = event.first;
    this.rows = event.rows;
  }
}
