import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { EquipmentStockService } from 'src/app/services/equipment-stock.service';
import { EquipmentStock } from 'src/app/Models/equipment-stock.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MaterialModule } from 'src/app/material.module';

@Component({
  selector: 'app-add-edit-equipment-stock',
  standalone: true,
  imports: [MatDialogModule, ReactiveFormsModule, MatFormFieldModule, CommonModule, MatInputModule,MatIconModule,MatButtonModule,MaterialModule],
  templateUrl: './add-edit-equipment-stock.component.html',
  styleUrls: ['./add-edit-equipment-stock.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class EquipmentStockModalComponent implements OnInit {
  stockForm!: FormGroup;
  isEditMode: boolean = false;
  fileName: string | null = null; 
  constructor(
    private dialogRef: MatDialogRef<EquipmentStockModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: EquipmentStock,
    private fb: FormBuilder,
    private stockService: EquipmentStockService
  ) {}

  ngOnInit(): void {
    this.isEditMode = !!this.data;
    this.initForm();
  }

  private initForm(): void {
    this.stockForm = this.fb.group({
      id: [this.data?.id || null],
      equipmentName: [this.data?.equipmentName || '', Validators.required],
      photo: [this.data?.photo || null, Validators.required],
    });
    
  }

  save(): void {
    if (this.stockForm.invalid) return;
  
    const stockData: EquipmentStock = { ...this.stockForm.value };
  
    if (this.isEditMode) {
      console.log('Updating Stock:', stockData);
      this.stockService.updateStock(this.data.id!, stockData).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => console.error('Error updating stock:', err),
      });
    } else {
      console.log('Adding Stock:', stockData);
      delete stockData.id; // Make sure id is not present when adding new stock
      this.stockService.addStock(stockData).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => console.error('Error adding stock:', err, 'Data Sent:', stockData),
      });
    }
  }
  async onFileSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.fileName = file.name;

      try {
        const compressedImageUrl = await this.compressImage(file);
        this.stockForm.get('photo')?.setValue(compressedImageUrl);
      } catch (error) {
        console.error('Error compressing image:', error);
      }
    }
  }
  private async compressImage(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const img = new Image();
      const reader = new FileReader();

      reader.onload = (e: ProgressEvent<FileReader>) => {
        if (e.target?.result) {
          img.src = e.target.result as string;
        }
      };

      img.onload = () => {
        const maxWidth = 450;
        const maxHeight = 450;

        const { width, height } = this.calculateAspectRatioFit(
          img.width,
          img.height,
          maxWidth,
          maxHeight
        );

        const canvas = document.createElement('canvas');
        const ctx = canvas.getContext('2d');

        if (!ctx) {
          reject(new Error('Could not create canvas context'));
          return;
        }

        canvas.width = width;
        canvas.height = height;
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.drawImage(img, 0, 0, width, height);

        const compressedImageUrl = canvas.toDataURL('image/png', 0.7);
        resolve(compressedImageUrl);
      };

      img.onerror = () => {
        reject(new Error('Failed to load image'));
      };

      reader.readAsDataURL(file);
    });
  }

  private calculateAspectRatioFit(
    srcWidth: number,
    srcHeight: number,
    maxWidth: number,
    maxHeight: number
  ): { width: number; height: number } {
    const ratio = Math.min(maxWidth / srcWidth, maxHeight / srcHeight);
    return {
      width: srcWidth * ratio,
      height: srcHeight * ratio,
    };
  }
  close(): void {
    this.dialogRef.close();
  }
}
