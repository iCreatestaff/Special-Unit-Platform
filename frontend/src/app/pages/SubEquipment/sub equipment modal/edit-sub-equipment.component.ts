import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { HttpClientModule } from '@angular/common/http';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { SubEquipmentService } from 'src/app/services/sub-equipment.service';

@Component({
  selector: 'app-edit-sub-equipment-dialog',
   standalone: true,
    imports: [
      CommonModule,
      MatCardModule,
      MatIconModule,
      MatSlideToggleModule,
      HttpClientModule,
      MatFormFieldModule,
      MatInputModule,
      MatCheckboxModule,
      FormsModule,
      ReactiveFormsModule,
      MatButtonModule,
      MatSelectModule,
      MatChipsModule,
      
      
    ],
  templateUrl: './edit-sub-equipment.component.html',
})
export class EditSubEquipmentDialogComponent {
  subEquipment: any = this.data.subEquipment;
  cycleNumber: number = this.subEquipment.cycleNumber;
  cycleUnit: string = this.subEquipment.cycleUnit;
  availableStatuses = [
    { value: 'bon_etat', label: 'Good condition' },
    { value: 'avant_panne', label: 'Before out of service' },
    { value: 'en_panne', label: 'Out of Service' }
  ];

  constructor(
    private subEquipmentService: SubEquipmentService,
    public dialogRef: MatDialogRef<EditSubEquipmentDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { subEquipment: any }
  ) {}
  ngOnInit(): void {
    // Separate cycle value into number and unit when the dialog is initialized
    this.separateCycle(this.subEquipment.cycle);
  }
  separateCycle(cycle: string): void {
    const parts = cycle.split(' '); // Split cycle into parts
    if (parts.length === 2) {
      this.cycleNumber = Number(parts[0]);  // Set cycle number
      this.cycleUnit = parts[1];             // Set cycle unit
    }
  }
  onCycleChange(): void {
    this.subEquipment.cycle = `${this.cycleNumber} ${this.cycleUnit}`;
     // Concatenate the number and unit into a single string
  }
 
  onSubmit(): void {
    this.onCycleChange();
    // Call the updateSubEquipment service with the id and updated data
    this.subEquipmentService.updateSubEquipment(this.subEquipment.id, this.subEquipment).subscribe({
      next: () => {
        this.dialogRef.close(true);  // Close the dialog and notify the parent component
      },
      error: (err) => {
        console.error('Error updating sub-equipment:', err);
        // Handle error (e.g., show a message to the user)
      }
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }
}