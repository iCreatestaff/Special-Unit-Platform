import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { EquipmentStockService } from 'src/app/services/equipment-stock.service';

@Component({
  selector: 'app-delete-equipment-stock',
  standalone:true,
  imports:[
    MatDialogModule  ,CommonModule,MatCardModule,MatIconModule,MatButtonModule
  ],
  templateUrl: './delete-equipment-stock.component.html',
  styleUrls: ['./delete-equipment-stock.component.css']
})
export class DeleteEquipmentStockModalComponent {
    constructor(
      public dialogRef: MatDialogRef<DeleteEquipmentStockModalComponent>,
      @Inject(MAT_DIALOG_DATA) public data: { id: number },
      private stockService: EquipmentStockService
    ) {}
  
    confirmDelete(): void {
      this.stockService.deleteStock(this.data.id).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => console.error('Error deleting stock:', err),
      });
    }
  
    close(): void {
      this.dialogRef.close();
    }
  }
