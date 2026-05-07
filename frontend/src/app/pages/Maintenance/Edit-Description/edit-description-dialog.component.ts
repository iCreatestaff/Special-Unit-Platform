// edit-maintenance-modal.component.ts
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatDialogModule } from '@angular/material/dialog';
import { Maintenance } from 'src/app/Models/maintenance.model';

@Component({
  selector: 'app-edit-maintenance-modal',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule
  ],
  template: `
    <div class="edit-modal-container">
      <h2 mat-dialog-title>Edit Maintenance</h2>
      <mat-dialog-content>
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Description</mat-label>
          <textarea matInput [(ngModel)]="maintenance.description" rows="4"></textarea>
        </mat-form-field>
      </mat-dialog-content>
      <mat-dialog-actions align="end">
        <button mat-button (click)="onCancel()">Cancel</button>
        <button mat-raised-button color="primary" (click)="onSave()">Save</button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [`
    .edit-modal-container {
      padding: 20px;
    }
    .full-width {
      width: 100%;
    }
    textarea {
      min-height: 100px;
    }
  `]
})
export class EditMaintenanceModalComponent {
  maintenance: Maintenance;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { maintenance: Maintenance },
    private dialogRef: MatDialogRef<EditMaintenanceModalComponent>
  ) {
    this.maintenance = data.maintenance;
  }

  onSave(): void {
    this.dialogRef.close(this.maintenance);
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}