import { Component, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TrainingService } from 'src/app/services/training.service';
import { AccountService } from 'src/app/services/account.service';
import { Account } from 'src/app/Models/account.model';
import * as L from 'leaflet';
import { CommonModule } from '@angular/common';
import { MatButtonModule, MatIconButton } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatStepperModule } from '@angular/material/stepper';
import { MatChip, MatChipsModule } from '@angular/material/chips';
import { MatStepper } from '@angular/material/stepper';

@Component({
  selector: 'app-training-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    ReactiveFormsModule,
    MatDatepickerModule,
    MatStepperModule,
    MatDialogModule,
    MatIconButton,
    MatIconModule,
    MatChipsModule
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './training-modal.component.html',
  styleUrls: ['./training-modal.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class TrainingModalComponent implements AfterViewInit {
  trainingForm: FormGroup;
  accounts: Account[] = [];
  map!: L.Map;
  marker!: L.Marker;
  
  timeOptions: string[] = this.generateTimeOptions();
  
  private customIcon = L.icon({
    iconUrl: 'assets/images/marker-icon.png',
    shadowUrl: 'assets/images/marker-shadow.png',
    iconSize: [25, 41],
    iconAnchor: [12, 41],
    popupAnchor: [1, -34],
    shadowSize: [41, 41]
  });

  constructor(
    public dialogRef: MatDialogRef<TrainingModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private fb: FormBuilder,
    private trainingService: TrainingService,
    private accountService: AccountService
  ) {
    this.trainingForm = this.createForm();
    if (this.data) {
      // If editing, load accounts immediately since we have all data
      this.loadAccounts();
    }
  }

  ngAfterViewInit(): void {
    this.initMap();
    if (this.data?.location) {
      this.setExistingLocationMarker();
    }
  }

  private createForm(): FormGroup {
    return this.fb.group({
      title: [this.data?.title || '', Validators.required],
      description: [this.data?.description || ''],
      dateRange: this.fb.group({
        start: [this.data?.startTime ? this.adjustTimeForLocal(this.data.startTime) : '', Validators.required],
        end: [this.data?.endTime ? this.adjustTimeForLocal(this.data.endTime) : '', Validators.required]
      }),
      startTimeOnly: [this.data?.startTime ? this.extractTime(this.data.startTime) : '', Validators.required],
      endTimeOnly: [this.data?.endTime ? this.extractTime(this.data.endTime) : '', Validators.required],
      location: [this.data?.location || '', Validators.required],
      assignedAccounts: [this.data?.assignedAccounts || [], Validators.required]
    });
  }

  onStepChange(event: any): void {
    if (event.selectedIndex === 1) { // When moving to the second step (Location & Participants)
      this.loadAccounts();
    }
  }

  onDateChange(): void {
    // When dates change, reload accounts if we have all required info
    if (this.trainingForm.get('startTimeOnly')?.value && this.trainingForm.get('endTimeOnly')?.value) {
      this.loadAccounts();
    }
  }

  onTimeChange(): void {
    // When times change, reload accounts if we have all required info
    if (this.trainingForm.get('dateRange.start')?.value && this.trainingForm.get('dateRange.end')?.value) {
      this.loadAccounts();
    }
  }

  getAccountName(accountId: string): string {
    const account = this.accounts.find(a => a.id.toString() === accountId);
    return account ? account.username : '';
  }
  
  removeParticipant(accountId: string): void {
    const current = this.trainingForm.get('assignedAccounts')?.value as string[];
    this.trainingForm.get('assignedAccounts')?.setValue(
      current.filter(id => id !== accountId)
    );
  }

  centerMap(): void {
    if (this.map) {
      this.map.setView([33.8869, 9.5375], 6);
    }
  }

  private loadAccounts(): void {
    const dateRange = this.trainingForm.get('dateRange')?.value;
    const startTime = this.trainingForm.get('startTimeOnly')?.value;
    const endTime = this.trainingForm.get('endTimeOnly')?.value;

    if (!dateRange?.start || !dateRange?.end || !startTime || !endTime) {
      console.warn('Date/time information missing - cannot load available agents');
      this.accounts = [];
      return;
    }

    const startDate = new Date(dateRange.start);
    const endDate = new Date(dateRange.end);

    // Add time to the date
    const [startHour, startMinute] = startTime.split(':').map(Number);
    const [endHour, endMinute] = endTime.split(':').map(Number);
    
    startDate.setHours(startHour, startMinute);
    endDate.setHours(endHour, endMinute);

    // If we're editing an existing training session, delete non-availability records first
    if (this.data?.id) {
      this.trainingService.deleteNonAvailability(this.data.id).subscribe({
        next: () => {
          // After deleting non-availability, load available accounts
          this.loadAvailableAccounts(startDate, endDate);
        },
        error: (err) => {
          console.error('Failed to delete non-availability records:', err);
          // Even if deletion fails, try to load available accounts
          this.loadAvailableAccounts(startDate, endDate);
        }
      });
    } else {
      // For new training sessions, just load available accounts
      this.loadAvailableAccounts(startDate, endDate);
    }
  }

  private loadAvailableAccounts(startDate: Date, endDate: Date): void {
    this.accountService.getAvailableAccounts(startDate.toISOString(), endDate.toISOString()).subscribe({
      next: (accounts) => {
        this.accounts = accounts.filter(account => account.role === 'Agent');
        
      },
      error: (err) => {
        console.error('Failed to load available agents:', err);
        this.accounts = [];
      }
    });
  }

  private initMap(): void {
    this.map = L.map('map').setView([33.8869, 9.5375], 6);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: 'Map data © OpenStreetMap contributors'
    }).addTo(this.map);

    this.map.on('click', (e: L.LeafletMouseEvent) => {
      const { lat, lng } = e.latlng;
      this.updateLocation(lat, lng);
    });
  }

  private updateLocation(lat: number, lng: number): void {
    this.trainingForm.get('location')?.setValue(`${lat},${lng}`);
    if (this.marker) {
      this.map.removeLayer(this.marker);
    }
    this.marker = L.marker([lat, lng], { icon: this.customIcon })
      .addTo(this.map)
      .bindPopup('Training Location')
      .openPopup();
  }

  private setExistingLocationMarker(): void {
    const [lat, lng] = this.data.location.split(',').map(Number);
    if (!isNaN(lat) && !isNaN(lng)) {
      this.marker = L.marker([lat, lng], { icon: this.customIcon })
        .addTo(this.map)
        .bindPopup('Training Location')
        .openPopup();
      this.map.setView([lat, lng], 13);
    }
  }

  private generateTimeOptions(): string[] {
    const options = [];
    for (let hour = 0; hour < 24; hour++) {
      for (let minute = 0; minute < 60; minute += 15) {
        const hourStr = hour.toString().padStart(2, '0');
        const minuteStr = minute.toString().padStart(2, '0');
        options.push(`${hourStr}:${minuteStr}`);
      }
    }
    return options;
  }

  adjustTimeForLocal(date: string): Date {
    const d = new Date(date);
    return new Date(d.getTime() - d.getTimezoneOffset() * 60000);
  }

  extractTime(datetime: string): string {
    const date = new Date(datetime);
    const localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
    return localDate.toISOString().substring(11, 16);
  }

  onSubmit(): void {
    if (this.trainingForm.invalid) {
      this.markFormGroupTouched(this.trainingForm);
      return;
    }

    const formValue = this.trainingForm.value;
    const payload = {
      id: this.data?.id,
      title: formValue.title,
      description: formValue.description,
      startTime: new Date(this.combineDateTime(formValue.dateRange.start, formValue.startTimeOnly)),
      endTime: new Date(this.combineDateTime(formValue.dateRange.end, formValue.endTimeOnly)),
      location: formValue.location,
      assignedAccounts: formValue.assignedAccounts
    };

    const operation = this.data?.id 
      ? this.trainingService.update(this.data.id, payload)
      : this.trainingService.create(payload);

    operation.subscribe({
      next: () => this.dialogRef.close(true),
      error: (err) => {
        console.error('Error saving training:', err);
        alert(`Failed to ${this.data?.id ? 'update' : 'create'} training. Please try again.`);
      }
    });
  }

  private combineDateTime(date: string | Date, time: string): string {
    const d = new Date(date);
    const [hours, minutes] = time.split(':');
    d.setHours(Number(hours));
    d.setMinutes(Number(minutes));
    return new Date(d.getTime() - d.getTimezoneOffset() * 60000).toISOString();
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }
}