import { Component, Inject } from '@angular/core';  
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';  
import { Equipment } from 'src/app/Models/equipment.model';  
import { CommonModule } from '@angular/common';  
import { FormsModule, ReactiveFormsModule } from '@angular/forms';  
import { MatButtonModule } from '@angular/material/button';  
import { MatCardModule } from '@angular/material/card';  
import { MatIconModule } from '@angular/material/icon';  
import { MatInputModule } from '@angular/material/input';  
import { MatDialogModule } from '@angular/material/dialog';  
import { MatFormFieldModule } from '@angular/material/form-field';  
import { MatCheckboxModule } from '@angular/material/checkbox';  
import { MatSlideToggleModule } from '@angular/material/slide-toggle';  
import { RouterModule } from '@angular/router';  
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { EquipmentService } from 'src/app/services/equipment.service';
import { MatTableDataSource } from '@angular/material/table';

@Component({  
  selector: 'app-equipment-modal',  
  standalone: true,  
  imports: [  
    CommonModule,  
    MatCardModule,  
    MatIconModule,  
    MatSlideToggleModule,  
    RouterModule,  
    MatFormFieldModule,  
    MatInputModule,  
    MatCheckboxModule,  
    MatDialogModule,  
    MatButtonModule,  
    FormsModule,  
    ReactiveFormsModule,  
    MatButtonToggleModule
  ],  
  templateUrl: './equipment-modal.component.html',  
  styleUrls: ['./equipment-modal.component.css'],  
})  
export class EquipmentModalComponent {  
  dataSource = new MatTableDataSource<Equipment>();
  equipments: Equipment[] = [];
  equipment: Equipment;  
  imageUrl: string = '';  
  fileName: string | null = null; 
  constructor(  private equipmentService: EquipmentService,
    public dialogRef: MatDialogRef<EquipmentModalComponent>,  
    @Inject(MAT_DIALOG_DATA) public data: Equipment | null  
  ) {  
    this.equipment = data   
      ? { ...data }   
      : { id: 0, name: '', type: '', availability: false, photo: '' };  
  }  
  getEquipments(): void {
    this.equipmentService.getEquipments().subscribe((data: Equipment[]) => {
      this.equipments = data;
      this.dataSource.data = this.equipments;
    });
  }
  onSave(): void {  
    this.dialogRef.close(this.equipment);  
    this.getEquipments();
  }  

  onCancel(): void {  
    this.dialogRef.close();  
    this.getEquipments();
  }  

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
    
      // Create a FileReader to read the selected file
      const reader = new FileReader();
      reader.onload = (e: ProgressEvent<FileReader>) => {
        const img = new Image();
        img.onload = () => {
          // Ensure the canvas dimensions maintain the aspect ratio
          const maxWidth = 200;
          const maxHeight = 200;
    
          let width = img.width;
          let height = img.height;
    
          // Calculate new dimensions based on max width/height
          if (width > height) {
            if (width > maxWidth) {
              height = (height * maxWidth) / width;
              width = maxWidth;
            }
          } else {
            if (height > maxHeight) {
              width = (width * maxHeight) / height;
              height = maxHeight;
            }
          }
    
          // Create a canvas element to resize the image
          const canvas = document.createElement('canvas');
          const ctx = canvas.getContext('2d');
          
          if (ctx) {
            // Set canvas dimensions
            canvas.width = width;
            canvas.height = height;
    
            // Ensure the canvas background is transparent
            ctx.clearRect(0, 0, canvas.width, canvas.height);  // Clear canvas to maintain transparency
    
            // Draw the image onto the canvas while keeping transparency intact
            ctx.drawImage(img, 0, 0, width, height);
    
            // Convert the canvas image to a compressed image URL (Base64)
            const compressedImageUrl = canvas.toDataURL('image/png', 0.7); // PNG for transparency
    
            // Set the photo to the Equipment object
            this.equipment.photo = compressedImageUrl;
            this.fileName = file.name; // Save the file name
          }
        };
    
        // Load image data
        img.src = e.target?.result as string;
      };
    
      // Read the file as a Data URL
      reader.readAsDataURL(file);
    }
  }
  

  onUrlEntered(event: Event): void {  
    const input = event.target as HTMLInputElement;  
    this.imageUrl = input.value; // Update the URL when entered.  
    if (this.imageUrl) {  
      this.equipment.photo = this.imageUrl; // Set the equipment photo to the URL  
    }  
  }  
}