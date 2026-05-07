import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SubEquipmentService } from 'src/app/services/sub-equipment.service';
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

@Component({
  selector: 'app-delete-confirmation-dialog',
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
  templateUrl: './delete-confirmation.component.html',
})
export class DeleteConfirmationDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<DeleteConfirmationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { id: number },
    private subEquipmentService: SubEquipmentService
  ) { console.log('Received data in dialog:', this.data);  // Check what data was passed
    }

    confirmDelete(): void {
      console.log('Deleting Sub-equipment ID:', this.data.id);  // Check ID before calling the delete service
    
      // Call the delete service to remove the sub-equipment
      this.subEquipmentService.deleteSubEquipment(this.data.id).subscribe({
        next: () => {
          this.dialogRef.close(true);  // Close the dialog and return 'true' if deletion is successful
        },
        error: (err) => {
          console.error('Error deleting sub-equipment:', err);
          this.dialogRef.close(false);  // Close the dialog without any action on error
        }
      });
    }

  cancel(): void {
    this.dialogRef.close(false); // Close without any action
  }
}
