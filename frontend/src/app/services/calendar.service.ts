import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CalendarService {
  events: { title: string; date: string }[] = [];

  // Method to add event to the calendar
  addEvent(event: { title: string; date: string }): void {
    this.events.push(event);
    console.log('Event added to calendar:', event);
  }

  // Method to get all events
  getEvents(): { title: string; date: string }[] {
    return this.events;
  }
}
