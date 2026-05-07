import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from 'src/app/material.module';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { AddEventDialogComponent } from '../add-event-dialog/add-event-dialog.component';
import { MaintenanceService } from 'src/app/services/maintenance.service';
import { MissionService } from 'src/app/services/mission.service';
import { TrainingService } from 'src/app/services/training.service';
import { EventModalComponent } from '../Event Modal/event-modal.component';
import { Maintenance } from 'src/app/Models/maintenance.model';
import { Mission } from 'src/app/Models/mission.model';
import { Training } from 'src/app/Models/training.model';
import { Subject, takeUntil } from 'rxjs';

interface CalendarEvent {
  type: 'maintenance' | 'mission' | 'training';
  data: Maintenance | Mission | Training;
}

interface CalendarCell {
  dateObj: Date;
  isToday: boolean;
  isCurrentMonth: boolean;
  events: CalendarEvent[];
}

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [MaterialModule, CommonModule, MatDialogModule],
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
})
export class CalendarComponent implements OnInit, OnDestroy {
  currentMonthDisplay: string = '';
  currentYear: string = '';
  selectedDate: Date = new Date();
  calendarCells: CalendarCell[] = [];
  weekDays: string[] = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
  private destroy$ = new Subject<void>();
  role: string = localStorage.getItem('role') || 'user';
  type: string = localStorage.getItem('type') || 'user';
  // Color palette for different event types
  private eventTypeColors = {
    
    maintenance: '#3f51b5', // indigo
    mission: '#ff4081',    // pink
    training: '#4caf50'    // green
  };

  constructor(
    private dialog: MatDialog,
    private maintenanceService: MaintenanceService,
    private missionService: MissionService,
    private trainingService: TrainingService,
    private cdRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.refreshCalendar();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private refreshCalendar(): void {
    this.updateMonthDisplay();
    this.generateCalendarGrid();
    this.loadAllEvents();
  }

  private updateMonthDisplay(): void {
    this.currentMonthDisplay = this.selectedDate.toLocaleString('default', {
      month: 'long',
      year: 'numeric'
    });
    this.currentYear = this.selectedDate.getFullYear().toString();
  }

  private generateCalendarGrid(): void {
    const year = this.selectedDate.getFullYear();
    const month = this.selectedDate.getMonth();

    const firstDayOfMonth = new Date(year, month, 1);
    const lastDayOfMonth = new Date(year, month + 1, 0);
    const daysInMonth = lastDayOfMonth.getDate();
    const firstWeekday = firstDayOfMonth.getDay();

    this.calendarCells = [];

    const todayStr = new Date().toDateString();

    // Previous month's tail
    const prevMonthLastDay = new Date(year, month, 0).getDate();
    for (let i = firstWeekday - 1; i >= 0; i--) {
      const date = new Date(year, month - 1, prevMonthLastDay - i);
      this.calendarCells.push({
        dateObj: date,
        isToday: date.toDateString() === todayStr,
        isCurrentMonth: false,
        events: []
      });
    }

    // Current month
    for (let i = 1; i <= daysInMonth; i++) {
      const date = new Date(year, month, i);
      this.calendarCells.push({
        dateObj: date,
        isToday: date.toDateString() === todayStr,
        isCurrentMonth: true,
        events: []
      });
    }

    // Next month's head
    const total = Math.ceil(this.calendarCells.length / 7) * 7;
    for (let i = 1; this.calendarCells.length < total; i++) {
      const date = new Date(year, month + 1, i);
      this.calendarCells.push({
        dateObj: date,
        isToday: date.toDateString() === todayStr,
        isCurrentMonth: false,
        events: []
      });
    }
  }

private loadAllEvents(): void {
  const isMaintenanceAgent = this.role === 'Agent' && this.type === 'maintenance';
  const isSuperAdmin = this.role === 'SuperAdmin';

  if (isMaintenanceAgent || isSuperAdmin) {
    this.loadMaintenances();
  }

  // Agents with non-maintenance type or any other users
  if (!isMaintenanceAgent) {
    this.loadMissions();
    this.loadTrainings();
  }
}


  private loadMaintenances(): void {
    this.maintenanceService.getAllMaintenances()
      .pipe(takeUntil(this.destroy$))
      .subscribe((maintenances: Maintenance[]) => {
        maintenances.forEach(m => {
          const event: CalendarEvent = { type: 'maintenance', data: m };
          this.addEventToCell(new Date(m.maintenanceDate), event);
        });
        this.cdRef.detectChanges();
      });
  }

  private loadMissions(): void {
    this.missionService.getMissions()
      .pipe(takeUntil(this.destroy$))
      .subscribe((missions: Mission[]) => {
        missions.forEach(m => {
          const event: CalendarEvent = { type: 'mission', data: m };
          const startDate = new Date(m.startTime);
          const endDate = new Date(m.endTime);
          this.addEventToCell(startDate, event); // or spread across dates if needed
        });
        this.cdRef.detectChanges();
      });
  }

  private loadTrainings(): void {
    this.trainingService.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe((trainings: Training[]) => {
        trainings.forEach(t => {
          const event: CalendarEvent = { type: 'training', data: t };
          const startDate = new Date(t.startTime);
          const endDate = new Date(t.endTime);
          this.addEventToCell(startDate, event); // can be extended to span multiple days
        });
        this.cdRef.detectChanges();
      });
  }

  private addEventToCell(date: Date, event: CalendarEvent): void {
    const dateStr = date.toDateString();
    const cell = this.calendarCells.find(c => c.dateObj.toDateString() === dateStr);
    if (cell) {
      cell.events.push(event);
    }
  }

 

  getEventColor(event: CalendarEvent): string {
    
    return this.eventTypeColors[event.type] || '#ccc';
  }

  getEventTitle(event: CalendarEvent): string {
    if ('title' in event.data && event.data.title) return event.data.title;
    if ('description' in event.data && event.data.description) return event.data.description;
    return 'Untitled';
  }
getEventTypeLabels(): { type: string, label: string, color: string }[] {
  const isMaintenanceAgent = this.role === 'Agent' && this.type === 'maintenance';
  const isSuperAdmin = this.role === 'SuperAdmin';

  if (isMaintenanceAgent || isSuperAdmin) {
    return [{ type: 'maintenance', label: 'Maintenance', color: this.eventTypeColors.maintenance }];
  }

  return [
    { type: 'mission', label: 'Mission', color: this.eventTypeColors.mission },
    { type: 'training', label: 'Training', color: this.eventTypeColors.training }
  ];
}

  // Rest of your existing methods (navigateToPreviousMonth, navigateToNextMonth, etc.)
  // ... keep all your existing methods but update them to work with CalendarEvent interface

  openEventDialog(cell: CalendarCell): void {
    if (!cell.events.length) return;

    this.dialog.open(EventModalComponent, {
      width: '80%',
      maxWidth: '1200px',
      height: '80%',
      maxHeight: '80vh',
      data: { 
        date: cell.dateObj, 
        events: cell.events,
        eventColors: this.eventTypeColors
      },
      panelClass: 'event-dialog'
    });
  }

  // Update your trackBy functions
  trackByDate(index: number, cell: CalendarCell): number {
    return cell.dateObj.getTime();
  }

  trackByEvent(index: number, event: CalendarEvent): string {
    return `${event.type}-${event.data.id}`;
  }
  navigateToPreviousMonth(): void {
    this.selectedDate = new Date(this.selectedDate.getFullYear(), this.selectedDate.getMonth() - 1, 1);
    this.refreshCalendar();
  }

  navigateToNextMonth(): void {
    this.selectedDate = new Date(this.selectedDate.getFullYear(), this.selectedDate.getMonth() + 1, 1);
    this.refreshCalendar();
  }

  goToToday(): void {
    this.selectedDate = new Date();
    this.refreshCalendar();
  }
}