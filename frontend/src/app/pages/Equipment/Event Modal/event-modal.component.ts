import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { RouterModule } from '@angular/router';
import { MaterialModule } from 'src/app/material.module';
import { Maintenance } from 'src/app/Models/maintenance.model';
import { Mission } from 'src/app/Models/mission.model';
import { Training } from 'src/app/Models/training.model';

type EventType = 'maintenance' | 'mission' | 'training';

interface CalendarEvent {
  type: EventType;
  data: Maintenance | Mission | Training;
}

interface EventTypeConfig {
  displayName: string;
  icon: string;
  color: string;
  secondaryColor: string;
}

@Component({
  selector: 'app-event-modal',
  standalone: true,
  imports: [
    MaterialModule,
    CommonModule,
    RouterModule,
    MatDialogModule,
  ],
  templateUrl: './event-modal.component.html',
  styleUrls: ['./event-modal.component.css']
})
export class EventModalComponent {
  private readonly eventTypeConfigs: Record<EventType, EventTypeConfig> = {
    maintenance: {
      displayName: 'Maintenance',
      icon: 'build',
      color: '#3f51b5',
      secondaryColor: '#e8eaf6'
    },
    mission: {
      displayName: 'Mission',
      icon: 'rocket',
      color: '#ff4081',
      secondaryColor: '#fce4ec'
    },
    training: {
      displayName: 'Training',
      icon: 'school',
      color: '#4caf50',
      secondaryColor: '#e8f5e9'
    }
  };

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: {
      date: Date;
      events: CalendarEvent[];
    },
    private dialogRef: MatDialogRef<EventModalComponent>
  ) {}

  closeDialog(): void {
    this.dialogRef.close();
  }

  getEventConfig(type: EventType): EventTypeConfig {
    return this.eventTypeConfigs[type] || {
      displayName: 'Event',
      icon: 'event',
      color: '#9e9e9e',
      secondaryColor: '#f5f5f5'
    };
  }

  getEventTitle(event: CalendarEvent): string {
    if (event.type === 'mission') {
      const mission = event.data as Mission;
      return mission.description 
        ? this.truncateText(mission.description, 50) 
        : 'Mission';
    }

    const eventData: any = event.data;
    return eventData.title || eventData.name || this.getEventConfig(event.type).displayName;
  }

  getEventTimeRange(event: CalendarEvent): string {
    let startDate: Date | null = null;
    let endDate: Date | null = null;

    switch(event.type) {
      case 'mission':
        const mission = event.data as Mission;
        startDate = new Date(mission.startTime);
        endDate = new Date(mission.endTime);
        break;
      case 'training':
        const training = event.data as Training;
        startDate = new Date(training.startTime);
        endDate = new Date(training.endTime);
        break;
      case 'maintenance':
        const maintenance = event.data as Maintenance;
        startDate = new Date(maintenance.maintenanceDate);
        endDate = new Date(maintenance.maintenanceEndDate);
        break;
    }

    if (!startDate || !endDate) return 'No time range available';

    const isSameDay = startDate.toDateString() === endDate.toDateString();
    const timeFormat: Intl.DateTimeFormatOptions = { 
      hour: '2-digit', 
      minute: '2-digit',
      hour12: true
    };

    if (isSameDay) {
      return `${startDate.toLocaleDateString()} • ${startDate.toLocaleTimeString([], timeFormat)} - ${endDate.toLocaleTimeString([], timeFormat)}`;
    }
    return `${startDate.toLocaleString()} - ${endDate.toLocaleString()}`;
  }

  getEventLocation(event: CalendarEvent): string {
    const eventData: any = event.data;
    return eventData.location || 
      (event.type === 'training' ? 'Training location' : 
       event.type === 'maintenance' ? 'Maintenance site' : 'Location not specified');
  }

  getEventDescription(event: CalendarEvent): string {
    const eventData: any = event.data;
    return eventData.description || 
      (event.type === 'training' ? 'Training session details' : 
       event.type === 'maintenance' ? 'Scheduled maintenance' : 'No description provided');
  }

  private truncateText(text: string, maxLength: number): string {
    return text.length > maxLength 
      ? `${text.substring(0, maxLength)}...` 
      : text;
  }

  trackByEvent(index: number, event: CalendarEvent): string {
    return `${event.type}-${JSON.stringify(event.data)}`;
  }
}