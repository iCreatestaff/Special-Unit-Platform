import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';

import { EquipmentWithQuantity, EquipmentStock } from 'src/app/Models/equipment-stock.model';
import { Equipment } from 'src/app/Models/equipment.model';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggle } from '@angular/material/slide-toggle';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MaterialModule } from 'src/app/material.module';
import { EquipmentStockService } from 'src/app/services/equipment-stock.service';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-create-equipment',
  templateUrl: './create-equipment.component.html',
  styleUrls: ['./create-equipment.component.css'],
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatCardModule,
    ReactiveFormsModule,
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MaterialModule,
    FormsModule,
    MatSelectModule
   
]
})
export class CreateEquipmentComponent implements OnInit {
  equipmentForm: FormGroup;
  fileName: string = '';
  isLoading = false;
  imageUrl: string = '';
  errorMessage: string = '';
  equipmentStocks: EquipmentStock[] = [];
  constructor(
    private fb: FormBuilder,
    private equipmentStockService: EquipmentStockService,
    private dialogRef: MatDialogRef<CreateEquipmentComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { equipment: Equipment; quantity: number }
  ) {
    
  }

  ngOnInit(): void {
    this.equipmentForm = this.fb.group({
      name: ['', Validators.required],
      type: ['', Validators.required],
      photo: ['', Validators.required],
      availability: [true],
      quantity: [this.data?.quantity || 1, [Validators.required, Validators.min(1)]],
     
    });
    this.getStockName();
  }

  getStockName(): void {
    this.equipmentStockService.fetchStock().subscribe(
      (equipmentStocks) => {
        console.log('Stock received in component:', equipmentStocks); // Vérifier les données
        if (equipmentStocks && equipmentStocks.length > 0) {
          this.equipmentStocks = equipmentStocks;
        } else {
          console.warn('No stock data received.');
        }
      },
      (error) => {
        console.error('Error fetching equipment', error);
      }
    );
  }
  
  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.compressImage(file).then((compressedImage) => {
        this.equipmentForm.patchValue({ photo: compressedImage });
        this.imageUrl = compressedImage;
        this.fileName = file.name;
      });
    }
  }

  compressImage(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = (event: any) => {
        const img = new Image();
        img.onload = () => {
          const canvas = document.createElement('canvas');
          const maxWidth = 450; // Maximum width for the compressed image
          const maxHeight = 450; // Maximum height for the compressed image
          let width = img.width;
          let height = img.height;

          // Calculate the new dimensions while maintaining the aspect ratio
          if (width > height) {
            if (width > maxWidth) {
              height *= maxWidth / width;
              width = maxWidth;
            }
          } else {
            if (height > maxHeight) {
              width *= maxHeight / height;
              height = maxHeight;
            }
          }

          // Set canvas dimensions
          canvas.width = width;
          canvas.height = height;

          // Draw the image on the canvas with the new dimensions
          const ctx = canvas.getContext('2d');
          if (ctx) {
            ctx.drawImage(img, 0, 0, width, height);

            // Convert the canvas content to a base64 string (png format with 70% quality)
            const compressedImage = canvas.toDataURL('image/png', 0.7);
            resolve(compressedImage);
          } else {
            reject(new Error('Could not get canvas context'));
          }
        };
        img.onerror = (error) => reject(error);
        img.src = event.target.result;
      };
      reader.onerror = (error) => reject(error);
      reader.readAsDataURL(file);
    });
  }

  
  onSubmit(): void {
    if (this.equipmentForm.valid) {
      const formData = this.equipmentForm.value;
  
      const equipmentWithQuantity: EquipmentWithQuantity = {
        equipment: {
          name: formData.name,
          type: formData.type,
          photo: formData.photo,
          availability: formData.availability,
          id: 0
        },
        quantity: formData.quantity,
      };
      console.log(formData)
      this.dialogRef.close(equipmentWithQuantity); // Pass the data back to the parent component
    } else {
      this.errorMessage = "Please fill in all required fields.";
    }
  }
  

  close(): void {
    this.dialogRef.close();
  }
}