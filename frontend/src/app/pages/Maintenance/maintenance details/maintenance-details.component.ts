import { CommonModule, DatePipe } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from 'src/app/material.module';
import { Maintenance, MaintenanceGrouped } from 'src/app/Models/maintenance.model';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MaintenanceDayModalComponent } from '../maintenance-modal/maintenance-modal.component';

@Component({
  selector: 'app-maintenance-modal',
  standalone: true,
  imports: [
    DatePipe,
    CommonModule,
    MaterialModule,
    MatIconModule,
    MatDialogModule,
    MatButtonModule,
    MaintenanceDayModalComponent
  ],
  templateUrl: './maintenance-details.component.html',
  styleUrls: ['./maintenance-details.component.css']
})
export class MaintenanceModalComponent {
  selectedDate: Date = new Date();
  weekdays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: MaintenanceGrouped,
    private dialogRef: MatDialogRef<MaintenanceModalComponent>,
    private dialog: MatDialog
  ) {}

  getMonthDays(): Date[] {
    const year = this.selectedDate.getFullYear();
    const month = this.selectedDate.getMonth();
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);

    const prevMonthDays = firstDay.getDay();
    const startDate = new Date(year, month, 1 - prevMonthDays);

    const days: Date[] = [];
    for (let d = new Date(startDate); d <= lastDay; d.setDate(d.getDate() + 1)) {
      days.push(new Date(d));
    }

    return days;
  }

  trackByDate(index: number, item: Date): string {
    return item.toISOString();
  }

  getMaintenancesForDate(date: Date): Maintenance[] {
    return this.data.maintenances.filter(m => {
      const mDate = new Date(m.maintenanceDate);
      return mDate.toDateString() === date.toDateString();
    });
  }

  openDayModal(date: Date) {
    const maintenances = this.getMaintenancesForDate(date);
    if (maintenances.length === 0) return;
    
    this.dialog.open(MaintenanceDayModalComponent, {
      data: {
        date,
        maintenances
      },
      width: '600px'
    });
  }

  prevMonth() {
    this.selectedDate = new Date(
      this.selectedDate.getFullYear(),
      this.selectedDate.getMonth() - 1,
      1
    );
  }

  nextMonth() {
    this.selectedDate = new Date(
      this.selectedDate.getFullYear(),
      this.selectedDate.getMonth() + 1,
      1
    );
  }

  isCurrentMonth(date: Date): boolean {
    return date.getMonth() === this.selectedDate.getMonth();
  }

  isToday(date: Date): boolean {
    const today = new Date();
    return date.toDateString() === today.toDateString();
  }

  closeDialog() {
    this.dialogRef.close();
  }
}
