import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

import { EquipmentService } from 'src/app/services/equipment.service';
import { Equipment } from 'src/app/Models/equipment.model';
import { EquipmentModalComponent } from '../equipment modal/equipment-modal';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatMenuModule } from '@angular/material/menu';
import { MatChipsModule } from '@angular/material/chips';
import { CdkDrag, CdkDropList } from '@angular/cdk/drag-drop';
import { MatButtonModule } from '@angular/material/button';
import { EquipmentDetailsComponent } from '../equiment details/equipment-details';
import { MatButtonToggle, MatButtonToggleGroup, MatButtonToggleModule } from '@angular/material/button-toggle';
import { DeleteEquipmentComponent } from '../delete modal/delete-modal.component';
import { EquipmentStock, EquipmentWithQuantity } from 'src/app/Models/equipment-stock.model';
import { CreateEquipmentComponent } from '../../EquipmentStock/Create equipment Qte/create-equipment.component';
import { UpdateEquipmentStockComponent } from '../../EquipmentStock/update-All-Equipment/update-equipment-stock.component';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { finalize, Subject, takeUntil } from 'rxjs';
import { EquipmentStockService } from 'src/app/services/equipment-stock.service';
import { PaginatorModule } from 'primeng/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';


@Component({
  selector: 'app-equipment-list',
  standalone: true,
    imports: [
      CommonModule,
      MatCardModule,
      MatIconModule,
      MatSlideToggleModule,
      RouterModule, 
      HttpClientModule, // Required for API calls
      MatFormFieldModule,
      MatInputModule,
      MatCheckboxModule,
      MatFormFieldModule,
      MatChipsModule,
      MatIconModule,
      MatCardModule,
      MatButtonToggleModule,
      FormsModule,
      ReactiveFormsModule,
      MatButtonModule,
      MatTableModule,
      MatChipsModule,
      FormsModule,
      MatMenuModule,
      ReactiveFormsModule,
      PaginatorModule,
      MatProgressSpinnerModule 
      
    ],
  templateUrl: './equipment-list.component.html',
  styleUrls:['./equipment-list.component.css'],
  encapsulation: ViewEncapsulation.Emulated,
})
export class EquipmentComponent implements OnInit, OnDestroy {
  quantity: number = 1; // Default quantity
  equipments: Equipment[] = [];
  displayedColumns: string[] = ['photo','name', 'type', 'availability', 'actions']; // Add this line 
  rows: number = 10; // or whatever number of items per page you want
  first: number = 0;
  totalRecords: number = 0;
  pagedEquipments: Equipment[] = [];
  loading = false;
  errorMessage = '';

  equipmentStock: EquipmentStock[] = [];
  private readonly destroy$ = new Subject<void>();
  private isDestroyed = false;

  constructor(private equipmentService: EquipmentService, public dialog: MatDialog,private stockService: EquipmentStockService,private cdr: ChangeDetectorRef,) {
    
  }

  ngOnInit(): void {
    this.refreshData();  
  }

  ngOnDestroy(): void {
    this.isDestroyed = true;
    this.destroy$.next();
    this.destroy$.complete();
  }

  refreshData(): void {
    this.getEquipments();
  }

  onPageChange(event: any): void {
    this.first = event.first;
    this.rows = event.rows;
    this.updatePagedEquipments();
  }
  

  updatePagedEquipments(): void {
    const start = this.first;
    const end = this.first + this.rows;
    this.pagedEquipments = this.equipments.slice(start, end);
    this.refreshView();
  }
  

  getEquipments(): void {
    this.loading = true;
    this.errorMessage = '';
    this.refreshView();

    this.equipmentService.getEquipments()
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.loading = false;
          this.refreshView();
        })
      )
      .subscribe({
        next: (data: Equipment[]) => {
          this.equipments = data ?? [];
          this.totalRecords = this.equipments.length;
          this.updatePagedEquipments();
        },
        error: (err) => {
          this.equipments = [];
          this.pagedEquipments = [];
          this.totalRecords = 0;
          this.errorMessage = 'Failed to load equipment';
          console.error('Error fetching equipments:', err);
          this.refreshView();
        }
      });
  }
  


  openEquipmentModal(equipment: Equipment | null): void {
    const dialogRef = this.dialog.open(EquipmentModalComponent, {
     
      width: '100%',        // Adjust the width as you want (80% of the screen width)
      maxWidth: '1200px',  // Optional: Set a max width in pixels
      height: '60%',       // Adjust the height as you want (60% of the screen height)
      maxHeight: '80vh', // Adjust height automatically
     // Custom CSS for centering
      data: equipment ? { ...equipment } : null,
    });
  
    dialogRef.afterClosed().subscribe((result: Equipment | null) => {
      if (!result) return;
      if (equipment) {
        console.log('updating equipment:',result)
        this.equipmentService.updateEquipment(result.id, result).subscribe(() => this.getEquipments());
      } else {
        this.equipmentService.createEquipment(result).subscribe(() => this.getEquipments());
      }
    });
    
  }
  openAddWithQuantityDialog(): void {
    const dialogRef = this.dialog.open(CreateEquipmentComponent, {
      width: '100%',
      maxWidth: '1200px',
      height: '60%',
      maxHeight: '80vh',
      data: { quantity: this.quantity,equipment: null }, // Passing the quantity to the dialog
    });

    dialogRef.afterClosed().subscribe((result: EquipmentWithQuantity | null) => {
      if (!result) return; // User closed without saving

      if (result.equipment && result.quantity) {
        this.equipmentService.createEquipmentWithQuantity(result).subscribe(() => {
          this.getEquipments(); // Refresh the list after creation
        });
      }
    });
  }
  
  openUpdateAllEquipmentStockModal(equipment: Equipment): void {
    const dialogRef = this.dialog.open(UpdateEquipmentStockComponent, {
      width: '100%',
      maxWidth: '1200px',
      height: '60%',
      maxHeight: '80vh',
      data: { equipment }, // Pass the equipment to be updated
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.getEquipments(); // Refresh the list after updating
      }
    });
  }

  openEquipmentDetails(id: number): void {
    const dialogRef = this.dialog.open(EquipmentDetailsComponent, {
      width: '100%',        // Adjust the width as you want (80% of the screen width)
      maxWidth: '1200px',  // Optional: Set a max width in pixels
      height: '70%',       // Adjust the height as you want (60% of the screen height)
      maxHeight: '80vh',
      data: { id },  // Ensure this is passed correctly as an object with the `id` key
     
     
    });
    dialogRef.afterClosed().subscribe((result: Equipment | null) => {
      this.refreshData(); 
    });
  }
  

  
  deleteEquipment(id: number | undefined): void {
      if (id === undefined) {
        console.error('Mission ID is undefined');
        return;
      }
  
      const dialogRef = this.dialog.open(DeleteEquipmentComponent, {
        data: { id },
      });
  
      dialogRef.afterClosed().subscribe((confirmed) => {
        if (confirmed) {
          this.equipmentService.deleteEquipment(id).subscribe(() => {
            this.getEquipments();
          });
        }
      });
    }

  // deleteEquipment(id: number): void {
  //   this.equipmentService.deleteEquipment(id).subscribe(() => {
  //     this.getEquipments(); // Refresh after deletion
  //   });
  // }

  onToggleAvailability(equipment: Equipment): void {
    // Toggle the availability status
    equipment.availability = !equipment.availability;
    // Update the backend with the new availability status
    this.equipmentService.updateEquipment(equipment.id, equipment).subscribe(() => {
      this.getEquipments();  // Refresh after update
    });
  }

  private refreshView(): void {
    if (!this.isDestroyed) {
      this.cdr.detectChanges();
    }
  }
  
  
}
