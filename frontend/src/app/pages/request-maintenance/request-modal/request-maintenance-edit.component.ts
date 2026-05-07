import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MaterialModule } from 'src/app/material.module'; // Adjust if needed
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule, MaterialModule,FormsModule],
  templateUrl: './request-maintenance-edit.component.html',
  styleUrls: ['./request-maintenance-edit.component.css']
})
export class ConfirmDialogComponent {
  rejectionReason: string = '';

  constructor(
    public dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { 
      request: any,
      newStatus: string,
    }
  ) {
  
  }

  onConfirm(): void {
    this.dialogRef.close({ 
      confirmed: true,
      rejectionReason: this.rejectionReason
    });
  }

  onCancel(): void {
    this.dialogRef.close({ confirmed: false });
  }
}
