import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  CUSTOM_ELEMENTS_SCHEMA,
  Inject,
  OnDestroy,
  OnInit,
  ViewEncapsulation,
} from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE, MatNativeDateModule, NativeDateAdapter } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { Subject, takeUntil } from 'rxjs';
import { NonAvailability } from 'src/app/Models/nonavailability.model';
import { NonAvailabilityService } from 'src/app/services/nonavailability.service';

export const MY_DATE_FORMATS = {
  parse: { dateInput: 'DD/MM/YYYY' },
  display: {
    dateInput: 'DD/MM/YYYY',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};

@Component({
  selector: 'app-nonavailability-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    ReactiveFormsModule,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './nonavailability-modal.component.html',
  styleUrls: ['./nonavailability-form.component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    { provide: DateAdapter, useClass: NativeDateAdapter },
    { provide: MAT_DATE_LOCALE, useValue: 'en-GB' },
    { provide: MAT_DATE_FORMATS, useValue: MY_DATE_FORMATS },
  ],
})
export class NonAvailabilityModalComponent implements OnInit, OnDestroy {
  nonAvailabilityForm: FormGroup;
  timeOptions: string[] = this.generateTimeOptions();
  existingPeriods: NonAvailability[] = [];
  loadingExisting = false;
  saving = false;
  deletingId: number | null = null;
  errorMessage = '';

  private readonly destroy$ = new Subject<void>();
  private isDestroyed = false;

  constructor(
    private fb: FormBuilder,
    private nonAvailabilityService: NonAvailabilityService,
    public dialogRef: MatDialogRef<NonAvailabilityModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { accountId: number },
    private cdr: ChangeDetectorRef
  ) {
    this.nonAvailabilityForm = this.fb.group({
      dateRange: this.fb.group({
        start: [null, Validators.required],
        end: [null, Validators.required],
      }),
      startTimeOnly: ['', Validators.required],
      endTimeOnly: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadExistingPeriods();
  }

  ngOnDestroy(): void {
    this.isDestroyed = true;
    this.destroy$.next();
    this.destroy$.complete();
  }

  get dateRangeGroup(): FormGroup {
    return this.nonAvailabilityForm.get('dateRange') as FormGroup;
  }

  get selectedStart(): Date | null {
    return this.buildDateTime(
      this.dateRangeGroup.get('start')?.value,
      this.nonAvailabilityForm.get('startTimeOnly')?.value
    );
  }

  get selectedEnd(): Date | null {
    return this.buildDateTime(
      this.dateRangeGroup.get('end')?.value,
      this.nonAvailabilityForm.get('endTimeOnly')?.value
    );
  }

  get hasInvalidRange(): boolean {
    const start = this.selectedStart;
    const end = this.selectedEnd;
    return !!start && !!end && start >= end;
  }

  get canSave(): boolean {
    return this.nonAvailabilityForm.valid && !this.hasInvalidRange && !this.saving;
  }

  loadExistingPeriods(): void {
    if (!this.data?.accountId) return;

    this.loadingExisting = true;
    this.errorMessage = '';
    this.refreshView();

    this.nonAvailabilityService.getNonAvailabilityByAccount(this.data.accountId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (periods) => {
          this.existingPeriods = (periods || []).sort(
            (a, b) => new Date(a.date1).getTime() - new Date(b.date1).getTime()
          );
          this.loadingExisting = false;
          this.refreshView();
        },
        error: (err) => {
          console.error('Error loading non-availability periods:', err);
          this.errorMessage = 'Failed to load existing non-availability periods.';
          this.loadingExisting = false;
          this.refreshView();
        },
      });
  }

  onSave(): void {
    if (this.nonAvailabilityForm.invalid || this.hasInvalidRange) {
      this.nonAvailabilityForm.markAllAsTouched();
      return;
    }

    const { dateRange, startTimeOnly, endTimeOnly } = this.nonAvailabilityForm.value;
    const payload: NonAvailability = {
      accountId: this.data.accountId,
      date1: this.convertToUTC(dateRange.start, startTimeOnly),
      date2: this.convertToUTC(dateRange.end, endTimeOnly),
    };

    this.saving = true;
    this.errorMessage = '';
    this.refreshView();

    this.nonAvailabilityService.createNonAvailability(payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: (err) => {
          console.error('Error creating non-availability:', err);
          this.errorMessage = err?.error || 'Failed to save non-availability.';
          this.saving = false;
          this.refreshView();
        },
      });
  }

  deletePeriod(period: NonAvailability): void {
    if (!period.id) return;

    const shouldDelete = window.confirm('Delete this non-availability period?');
    if (!shouldDelete) return;

    this.deletingId = period.id;
    this.errorMessage = '';
    this.refreshView();

    this.nonAvailabilityService.deleteNonAvailability(period.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.existingPeriods = this.existingPeriods.filter(item => item.id !== period.id);
          this.deletingId = null;
          this.refreshView();
        },
        error: (err) => {
          console.error('Error deleting non-availability:', err);
          this.errorMessage = 'Failed to delete non-availability period.';
          this.deletingId = null;
          this.refreshView();
        },
      });
  }

  formatPeriod(period: NonAvailability): string {
    const start = new Date(period.date1);
    const end = new Date(period.date2);

    if (Number.isNaN(start.getTime()) || Number.isNaN(end.getTime())) {
      return 'Invalid period';
    }

    return `${start.toLocaleString()} - ${end.toLocaleString()}`;
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  private buildDateTime(date: Date | string | null, time: string): Date | null {
    if (!date || !time) return null;

    let formattedDate: string;
    if (date instanceof Date) {
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const day = String(date.getDate()).padStart(2, '0');
      formattedDate = `${year}-${month}-${day}`;
    } else {
      formattedDate = date.split('/').reverse().join('-');
    }

    const parsed = new Date(`${formattedDate}T${time}:00`);
    return Number.isNaN(parsed.getTime()) ? null : parsed;
  }

  private convertToUTC(date: Date | string, time: string): string {
    return this.buildDateTime(date, time)?.toISOString() || '';
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

  private refreshView(): void {
    if (!this.isDestroyed) {
      this.cdr.detectChanges();
    }
  }
}
