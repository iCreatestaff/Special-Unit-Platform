import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { SubEquipment } from 'src/app/Models/sub-equipment.model';
import { SubEquipmentService } from 'src/app/services/sub-equipment.service';
import { EquipmentService } from 'src/app/services/equipment.service';
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
import { MatSnackBar } from '@angular/material/snack-bar'; // Add MatSnackBar for notifications

@Component({
  selector: 'app-sub-equipment-add',
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
  templateUrl: './sub-equipment-add.component.html',
  styleUrls: ['./sub-equipment-add.component.css']
})
export class SubEquipmentAddComponent {
  subEquipment: SubEquipment = {
    id: 0,
    name: '',
    cycle: '',
    status: 'bon_etat',
    equipmentId: 0,
    creationDate: new Date().toISOString() // Add creationDate with current date
  };
  
  availableStatuses = [
    { value: 'bon_etat', label: 'Bon état' },
    { value: 'avant_panne', label: 'Avant panne' },
    { value: 'en_panne', label: 'En panne' }
  ];
  
  cycleNumber: number | null = null;
  cycleUnit: string = 'year';

  constructor(
    private subEquipmentService: SubEquipmentService,
    private router: Router,
    private equipmentService: EquipmentService,
    private snackBar: MatSnackBar // Inject MatSnackBar for notifications
  ) {}

  ngOnInit(): void {
    const equipmentId = history.state?.equipmentId;
    if (equipmentId) {
      this.subEquipment.equipmentId = equipmentId;
    } else {
      console.error('No equipmentId passed!');
      this.snackBar.open('No equipment ID passed!', 'Close', {
        duration: 3000,
        panelClass: ['error-snackbar']
      });
    }
  }

  // Handle cycle concatenation
  onCycleChange(): void {
    if (this.cycleNumber && this.cycleUnit) {
      this.subEquipment.cycle = `${this.cycleNumber} ${this.cycleUnit}`;
      console.log('Cycle:', this.subEquipment.cycle);  // Log the concatenated cycle
    }
  }

  // Submit the sub-equipment data
  onSubmit(): void {
    if (this.subEquipment.equipmentId) {
      console.log('Sub-Equipment JSON:', JSON.stringify(this.subEquipment)); // Log the JSON format of the sub-equipment
      this.subEquipmentService.createSubEquipment(this.subEquipment).subscribe({
        next: () => {
          this.snackBar.open('Sub-Equipment created successfully!', 'Close', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          this.router.navigate(['/Equipment/equipment-list']);
        },
        error: (err) => {
          console.error('Error creating sub-equipment:', err);
          this.snackBar.open('Error creating sub-equipment. Please try again.', 'Close', {
            duration: 3000,
            panelClass: ['error-snackbar']
          });
        }
      });
    } else {
      console.error('Equipment ID or Cycle is missing');
      this.snackBar.open('Please ensure all fields are filled correctly!', 'Close', {
        duration: 3000,
        panelClass: ['error-snackbar']
      });
    }
  }

  // Cancel the process and navigate back
  cancel(): void {
    this.router.navigate(['/Equipment/equipment-list']);
  }
}
