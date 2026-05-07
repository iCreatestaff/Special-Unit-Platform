import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { EquipmentStock, EquipmentWithQuantity } from 'src/app/Models/equipment-stock.model';
import { EquipmentStockService } from 'src/app/services/equipment-stock.service';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { EquipmentStockModalComponent } from '../modal equipmentStock/add-edit-equipment-stock.component';
import { DeleteEquipmentStockModalComponent } from '../delete confirmation/delete-equipment-stock.component';
import { CreateEquipmentComponent } from '../Create equipment Qte/create-equipment.component';
import { MatButtonModule } from '@angular/material/button';
import { UpdateEquipmentStockComponent } from '../update-All-Equipment/update-equipment-stock.component';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { catchError, finalize, of } from 'rxjs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PaginatorModule } from 'primeng/paginator';

@Component({
  selector: 'app-equipment-stock',
  standalone:true,
  imports:[
    PaginatorModule,MatIconModule ,CommonModule,MatCardModule ,MatTableModule,MatButtonModule,  ScrollingModule ,MatPaginator ,MatProgressSpinnerModule
  ],
  templateUrl: './equipment-stock.component.html',
  styleUrls: ['./equipment-stock.component.css']
})
export class EquipmentStockListComponent implements OnInit {
  equipmentStock: EquipmentStock[] = [];
  dataSource: MatTableDataSource<EquipmentStock> = new MatTableDataSource<EquipmentStock>();

  displayedColumns: string[] = ['photo', 'equipmentName', 'quantity', 'actions'];
  isLoading = true;
 
  first = 0;
  rows = 10;
  totalRecords = 0;
      @ViewChild(MatPaginator) paginator: MatPaginator;
    constructor(private stockService: EquipmentStockService, private dialog: MatDialog,private cdr: ChangeDetectorRef) {
     
    }
  
    ngOnInit(): void {
      this.loadEquipmentStock();
    }
    loadEquipmentStock(): void {
      this.isLoading = true;
      this.stockService.fetchStock().pipe(
        catchError((error: any) => {
          console.error('Error loading equipment stock:', error);
          return of([]);
        }),
        finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        })
      ).subscribe((data: EquipmentStock[]) => {
        this.equipmentStock = data;
        this.totalRecords = data.length;
        this.updateDataSource();
      });
    }
  
    updateDataSource(): void {
      const paginatedData = this.equipmentStock.slice(this.first, this.first + this.rows);
      this.dataSource.data = paginatedData;
    }
  
    onPageChange(event: any): void {
      this.first = event.first;
      this.rows = event.rows;
      this.updateDataSource();
    }
    ngAfterViewInit() {
      this.dataSource.paginator = this.paginator;
    }
   
    openAddEditModal(stock?: EquipmentStock): void {
      const dialogRef = this.dialog.open(EquipmentStockModalComponent, {
        width: '600px',
        data: stock ? { ...stock } : null,
       
      });
  
      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          this.loadEquipmentStock();
        }
      });
    }
  
    openUpdateModal(id: number): void {
      const dialogRef = this.dialog.open(UpdateEquipmentStockComponent, {
        maxWidth: '1200px',
        width:'100%',
        height:'80%',
        maxHeight: '80vh',
        data: { id },
    
      });
  
      dialogRef.componentInstance.refreshParent.subscribe(() => {
        this.loadEquipmentStock();
      });
    }
  
    openDeleteModal(id: number): void {
      const dialogRef = this.dialog.open(DeleteEquipmentStockModalComponent, {
        width: '500px',
        data: { id },
        
      });
  
      dialogRef.afterClosed().subscribe((confirmed) => {
        if (confirmed) {
          this.loadEquipmentStock();
        }
      });
    }
  }
