import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Training } from 'src/app/Models/training.model';
import { TrainingService } from 'src/app/services/training.service';
import { TrainingModalComponent } from '../training_modal/training-modal.component';
import { ConfirmationModalComponent } from '../confirmation_delete/confirmtion-modal.component';
import { MaterialModule } from 'src/app/material.module';
import { CommonModule } from '@angular/common';
import { MatIconButton } from '@angular/material/button';
import { PageEvent } from '@angular/material/paginator';
import { PaginatorModule } from 'primeng/paginator';
import { TrainingDetailsComponent } from '../training details/training-details.component';


@Component({
  selector: 'app-training-list',
  standalone:true,
  imports:[MaterialModule,CommonModule,MatIconButton,PaginatorModule],
  templateUrl: './training-list.component.html',
  styleUrls: ['./training-list.component.css']
})
export class TrainingListComponent implements OnInit {
  trainings: Training[] = [];
  paginatedTrainings: Training[] = [];
  first = 0; // index of first record in current page
  rows = 5; // rows per page
  totalRecords = 0;
  constructor(
    private trainingService: TrainingService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadTrainings();
  }

   loadTrainings(): void {
    this.trainingService.getAll().subscribe((data) => {
      this.trainings = data;
      this.totalRecords = data.length;
      this.updatePaginatedTrainings();
    });
  }

  updatePaginatedTrainings(): void {
    const start = this.first;
    const end = this.first + this.rows;
    this.paginatedTrainings = this.trainings.slice(start, end);
  }

  onPageChange(event: any): void {
    this.first = event.first;
    this.rows = event.rows;
    this.updatePaginatedTrainings();
  }
  openAddModal(): void {
    const dialogRef = this.dialog.open(TrainingModalComponent, {
      width: '90%',
      maxWidth: '1200px',
      height: '65%',
      maxHeight: '80vh',
      data: null,
      autoFocus: false,
      panelClass: 'custom-modalbox'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadTrainings();
    });
  }

  openEditModal(training: Training): void {
    const dialogRef = this.dialog.open(TrainingModalComponent, {
      width: '90%',
      maxWidth: '1200px',
      height: '65%',
      maxHeight: '80vh',
      autoFocus: false,
      panelClass: 'custom-modalbox',
      data: training
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadTrainings();
    });
  }
  openConsultModal(training: Training): void {
  this.dialog.open(TrainingDetailsComponent, {
    width: '100%',
      maxWidth: '1200px',
      height: '100%',
      maxHeight: '80vh',
    data: training,
    autoFocus: false,
    panelClass: 'custom-modalbox'
  });
}
  openDeleteModal(id: number): void {
    const dialogRef = this.dialog.open(ConfirmationModalComponent, {
      width: '600px',
      data: { id },
      autoFocus: false,
      panelClass: 'custom-modalbox'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadTrainings();
    });
  }
}
