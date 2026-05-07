import { ChangeDetectorRef, Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { Maintenance } from 'src/app/Models/maintenance.model';
import { CommonModule } from '@angular/common';
import { MaterialModule } from 'src/app/material.module';
import { MaintenanceService } from 'src/app/services/maintenance.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { EditMaintenanceModalComponent } from '../Edit-Description/edit-description-dialog.component';

@Component({
  selector: 'app-maintenance-day-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MaterialModule
  ],
  templateUrl: './maintenance-modal.component.html',
  styleUrls: ['./maintenance-modal.component.css']
})
export class MaintenanceDayModalComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { date: Date; maintenances: Maintenance[] },
    private dialogRef: MatDialogRef<MaintenanceDayModalComponent>,
    private maintenanceService: MaintenanceService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    private cdr: ChangeDetectorRef
  ) {}

  getStatusClass(status: string | null | undefined): string {
    return status ? status.toLowerCase() : 'unknown';
  }

  getCompletionPercentage(): number {
    if (!this.data.maintenances.length) return 0;
    const completed = this.data.maintenances.filter(m => m.status === 'Done').length;
    return (completed / this.data.maintenances.length) * 100;
  }

  getCompletedCount(): number {
    return this.data.maintenances.filter(m => m.status === 'Done').length;
  }

  isMaintenanceDue(maintenance: Maintenance): boolean {
    const now = new Date();
    return new Date(maintenance.maintenanceDate) <= now;
  }

  toggleStatus(maintenance: Maintenance): void {
    const updatedMaintenance: Maintenance = {
      ...maintenance,
      status: 'Done'
    };
  
    this.maintenanceService.editMaintenance(maintenance.id, updatedMaintenance).subscribe({
      next: () => {
        maintenance.status = 'Done';
        this.showSnackbar('Maintenance marked as completed', 'success');
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error updating status:', err);
        this.showSnackbar('Failed to update maintenance status', 'error');
      }
    });
  }

 
  private showSnackbar(message: string, type: 'success' | 'error'): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      panelClass: [`${type}-snackbar`],
      horizontalPosition: 'right',
      verticalPosition: 'top'
    });
  }

  close(): void {
    this.dialogRef.close();
  }
   openEditModal(maintenance: Maintenance): void {
    const dialogRef = this.dialog.open(EditMaintenanceModalComponent, {
      width: '500px',
      data: { maintenance: { ...maintenance } }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.maintenanceService.editMaintenance(result.id, result).subscribe({
          next: () => {
            const index = this.data.maintenances.findIndex(m => m.id === result.id);
            if (index !== -1) {
              this.data.maintenances[index] = result;
            }
            this.showSnackbar('Maintenance updated successfully', 'success');
            this.cdr.detectChanges();
          },
          error: (err) => {
            console.error('Error updating maintenance:', err);
            this.showSnackbar('Failed to update maintenance', 'error');
          }
        });
      }
    });
  }
}
