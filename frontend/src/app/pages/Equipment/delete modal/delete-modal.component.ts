import { Component, Inject } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MaterialModule } from 'src/app/material.module';

@Component({
    selector: 'app-confirm-dialog',
    standalone:true,
    imports: [
        MatDialogModule,
        MaterialModule,
    ],
    templateUrl: './delete-modal.component.html',
    styleUrls: ['./delete-modal.component.css']
})
export class DeleteEquipmentComponent {
  constructor(public dialogRef: MatDialogRef<DeleteEquipmentComponent>, @Inject(MAT_DIALOG_DATA) public data: any) {}

  confirmDelete(): void {
    this.dialogRef.close(true);
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
