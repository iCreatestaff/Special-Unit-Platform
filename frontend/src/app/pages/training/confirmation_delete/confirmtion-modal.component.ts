import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from 'src/app/material.module';
import { TrainingService } from 'src/app/services/training.service';


@Component({
  selector: 'app-confirmation-modal',
  standalone:true,
  imports:[MaterialModule],
  templateUrl: './confirmation-modal.component.html',
  styleUrls: ['./confirmation-modal.component.css']
})
export class ConfirmationModalComponent {
  constructor(
    public dialogRef: MatDialogRef<ConfirmationModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { id: number },
    private trainingService: TrainingService
  ) {}

  confirmDelete(): void {
    this.trainingService.delete(this.data.id).subscribe(() => this.dialogRef.close(true));
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
