import { ChangeDetectorRef, Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { EquipmentService } from '../../../services/equipment.service';
import { Equipment } from '../../../Models/equipment.model';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { SubEquipmentService } from 'src/app/services/sub-equipment.service';
import { DeleteConfirmationDialogComponent } from '../../SubEquipment/sub equipment delete/delete-confirmation.component';
import { SubEquipment } from 'src/app/Models/sub-equipment.model';
import { EditSubEquipmentDialogComponent } from '../../SubEquipment/sub equipment modal/edit-sub-equipment.component';
import { EquipmentStockService } from 'src/app/services/equipment-stock.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-equipment-details',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule,
    MatListModule,
    MatButtonModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './equipment-details.component.html',
  styleUrls: ['./equipment-details.component.css']
})
export class EquipmentDetailsComponent implements OnInit, OnDestroy {
  equipment: Equipment | null = null; // Initialize as null to prevent undefined errors
  dataSource: any[] = [];
  loading = false;
  errorMessage = '';
  @Input() equipmentId!: number;
   availableStatuses = [
    { value: 'bon_etat', label: 'Good condition' },
    { value: 'avant_panne', label: 'Before out of service' },
    { value: 'en_panne', label: 'Out of Service' }
  ];
  constructor(
    private dialog: MatDialog,
    private router: Router,
    private subEquimentService : SubEquipmentService,
    private equipmentService: EquipmentService,private equipmentstockService: EquipmentStockService,
    public dialogRef: MatDialogRef<EquipmentDetailsComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { id: number }, // Expecting ID to be passed to the dialog
    private cdr: ChangeDetectorRef
  ) { }

  private readonly destroy$ = new Subject<void>();
  private isDestroyed = false;

  ngOnInit(): void {
    console.log('Received ID:', this.data?.id);  // Log the ID received
    if (this.data?.id) {
      this.loadEquipment(this.data.id);
    } else {
      this.errorMessage = 'Equipment ID is missing.';
      this.refreshView();
    }
  }

  ngOnDestroy(): void {
    this.isDestroyed = true;
    this.destroy$.next();
    this.destroy$.complete();
  }
 
  
  loadEquipment(id: number): void {
    console.log('Fetching equipment for ID:', id);  // Log the ID before fetching
    this.loading = true;
    this.errorMessage = '';
    this.dataSource = [];
    this.refreshView();

    this.equipmentService.getEquipmentById(id)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.loading = false;
          this.refreshView();
        })
      )
      .subscribe({
      next: (response) => {
        console.log('Equipment data received:', response.photo);  // Log the response
        this.equipment = response;
        this.dataSource = response.subEquipments || [];
        this.refreshView();
      },
      error: (err) => {
        this.equipment = null;
        this.errorMessage = 'Failed to load equipment details.';
        console.error('Error fetching equipment:', err);
        this.refreshView();
      }
    });
  }
  navigateToAddSubEquipment() {
    this.router.navigate(['/SubEquipment/add-sub-equipment'], {
      state: { equipmentId: this.equipment?.id } // Pass the equipmentId
    });
    this.closeDialog()
  }
  closeDialog(): void {
    this.dialogRef.close();
  }
  openEditDialog(sub: SubEquipment): void {
    const dialogRef = this.dialog.open(EditSubEquipmentDialogComponent, {
      width: 'auto',
      height: 'auto',
      data: { subEquipment: sub }
    });
  
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Refresh the equipment details after the edit is successful
        if (this.equipment?.id) {
          this.loadEquipment(this.equipment.id);
        }
      
      }
    });
  }
  
  
  openDeleteDialog(sub: any): void {
    console.log('Sub-equipment ID before opening dialog:', sub.id); // Ensure ID is correct
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      data: { id: sub.id }  // Ensure we're passing the correct 'id' here
    });
  
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (this.equipment?.id) {
          this.loadEquipment(this.equipment.id);  // Reload equipment if delete is successful
        }
      }
    });
  }
  getStatusLabel(status: string): string {
  const found = this.availableStatuses.find(s => s.value === status);
  return found ? found.label : status;
}

private refreshView(): void {
  if (!this.isDestroyed) {
    this.cdr.detectChanges();
  }
}
  
  
  
  
}
