import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Inject,
  OnInit,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
} from '@angular/forms';
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialog,
} from '@angular/material/dialog';
import { EquipmentStockService } from 'src/app/services/equipment-stock.service';
import { Equipment } from 'src/app/Models/equipment.model';
import { MaterialModule } from 'src/app/material.module';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { EquipmentStock } from 'src/app/Models/equipment-stock.model';
import { EquipmentService } from 'src/app/services/equipment.service';
import { EquipmentDetailsComponent } from '../../Equipment/equiment details/equipment-details';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { EquipmentModalComponent } from '../../Equipment/equipment modal/equipment-modal';
import { DeleteEquipmentComponent } from '../../Equipment/delete modal/delete-modal.component';
import { UpdateAllEquipmentComponent } from '../updateAll/update-all-equipment.component';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { ViewChild } from '@angular/core';

@Component({
  selector: 'app-update-equipment-stock',
  standalone: true,
  imports: [
    MaterialModule,
    MatButtonModule,
    ReactiveFormsModule,
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
  ],
  templateUrl: './update-equipment-stock.component.html',
  styleUrls: ['./update-equipment-stock.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class UpdateEquipmentStockComponent implements OnInit {
  @Output() refreshParent = new EventEmitter<void>();
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  displayedColumns: string[] = ['photo', 'name', 'type', 'availability', 'actions'];
  dataSource = new MatTableDataSource<Equipment>([]);
  isLoading = false;
  pageSize = 5;
  currentPage = 0;
  totalPages = 0;
  fullEquipmentList: Equipment[] = []; // hold full list for pagination

  constructor(
    private fb: FormBuilder,
    private equipmentStockService: EquipmentStockService,
    private equipmentService: EquipmentService,
    public dialog: MatDialog,
    private dialogRef: MatDialogRef<UpdateEquipmentStockComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { id: number },
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.dataSource.paginator = this.paginator; // <--- assign once here
    if (this.data?.id) {
      this.getEquipmentsStock(this.data.id);
    } else {
      console.error('Error: Stock ID is undefined.');
    }
  }
  
  
  getEquipmentsStock(id: number): void {
    this.isLoading = true;
    this.equipmentStockService.getStockById(id).subscribe({
      next: (data: EquipmentStock) => {
        const enriched = (data.equipments ?? []).map(equipment => ({
          ...equipment,
         
        }));
        this.fullEquipmentList = enriched;
        this.totalPages = Math.ceil(enriched.length / this.pageSize);
        this.updateDataSource(); // slice for first page
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error fetching equipment stock:', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }
  
  onPageChange(event: PageEvent): void {
    this.currentPage = event.pageIndex;
    this.updateDataSource();
  }
  
  
  updateDataSource(): void {
    const startIndex = this.currentPage * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    const paginatedData = this.fullEquipmentList.slice(startIndex, endIndex);
    this.dataSource.data = paginatedData;
  }
  


  openEquipmentModal(equipment: Equipment | null): void {
    const dialogRef = this.dialog.open(EquipmentModalComponent, {
      width: '100%',
      maxWidth: '1200px',
      height: '60%',
      maxHeight: '80vh',
      data: equipment ? { ...equipment } : null,
    });

    dialogRef.afterClosed().subscribe((result: Equipment | null) => {
      if (!result) return;
      if (equipment) {
        this.equipmentService.updateEquipment(result.id, result).subscribe({
          next: () => this.getEquipmentsStock(this.data.id),
          error: (err) => console.error('Error updating equipment:', err),
        });
      } else {
        this.equipmentService.createEquipment(result).subscribe({
          next: () => this.getEquipmentsStock(this.data.id),
          error: (err) => console.error('Error creating equipment:', err),
        });
      }
    });
  }

  openEquipmentDetails(id: number): void {
    const dialogRef = this.dialog.open(EquipmentDetailsComponent, {
      width: '100%',
      maxWidth: '1300px',
      height: '80%',
      maxHeight: '80vh',
      data: { id },
    });

    dialogRef.afterClosed().subscribe(() => {
      this.getEquipmentsStock(this.data.id);
    });
  }

  deleteEquipment(id: number | undefined): void {
    if (id === undefined) return;
    const dialogRef = this.dialog.open(DeleteEquipmentComponent, {
      data: { id },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.equipmentService.deleteEquipment(id).subscribe({
          next: () => this.getEquipmentsStock(this.data.id),
          error: (err) => console.error('Error deleting equipment:', err),
        });
      }
    });
  }

  selectedEquipment(): Equipment | null {
    const stock = this.dataSource.data;
    const withSubs = stock.filter(e => e.subEquipments && e.subEquipments.length > 0);
    const pool = withSubs.length > 0 ? withSubs : stock;
    if (pool.length === 0) return null;

    const randomIndex = Math.floor(Math.random() * pool.length);
    return { ...pool[randomIndex], subEquipments: pool[randomIndex].subEquipments || [] };
  }

  openUpdateAllModal(): void {
    const equipment = this.selectedEquipment();
    if (equipment) {
      const dialogRef = this.dialog.open(UpdateAllEquipmentComponent, {
        width: '100%',
        maxWidth: '1200px',
        height: '90%',
        maxHeight: '80vh',
        data: { equipment },
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          this.getEquipmentsStock(this.data.id);
        }
      });
    } else {
      console.log('No equipment selected');
    }
  }

  onChangesSaved(): void {
    this.refreshParent.emit();
    this.dialogRef.close();
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
