import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from 'src/app/material.module';

@Component({
  selector: 'app-add-event-dialog',
   standalone: true,
    imports: [
      MaterialModule,
      MatDialogModule,
      CommonModule,
      FormsModule,
      MatDatepickerModule,
    MatNativeDateModule,
    ],
   
  templateUrl: './add-event-dialog.component.html',
  styleUrls: ['./add-event-dialog.component.css'],
})
export class AddEventDialogComponent {
  eventTitle: string = '';
  eventDate: string = '';
    
  constructor(
    public dialogRef: MatDialogRef<AddEventDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave(): void {
    const event = {
      title: this.eventTitle,
      date: this.eventDate,
    };
   
    this.dialogRef.close(event);
  }
  
}