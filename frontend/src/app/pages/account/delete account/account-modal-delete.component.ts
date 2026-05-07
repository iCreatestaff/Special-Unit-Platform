import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MaterialModule } from 'src/app/material.module';

@Component({
  selector: 'app-account-modal-delete',
  standalone:true,
  imports:[
    MatIconModule ,
    MaterialModule
  ],
  templateUrl: './account-modal-delete.component.html',
  styleUrls:['./account-modal-delete.component.css']
})
export class AccountModalDeleteComponent {
  constructor(
    public dialogRef: MatDialogRef<AccountModalDeleteComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { accountId: number }
  ) {}

  onDelete(): void {
    this.dialogRef.close(true);  // Confirm deletion
  }

  onCancel(): void {
    this.dialogRef.close(false);  // Cancel deletion
  }
}
