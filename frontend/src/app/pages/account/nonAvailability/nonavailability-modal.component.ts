import { Component, Inject, ViewEncapsulation, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { NonAvailabilityService } from 'src/app/services/nonavailability.service';
import { NonAvailability } from 'src/app/Models/nonavailability.model';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, DateAdapter, MAT_DATE_LOCALE, MAT_DATE_FORMATS, NativeDateAdapter } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';

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
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    NgxMaterialTimepickerModule,
    ReactiveFormsModule,
    MatIconModule, MatSelectModule,

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
export class NonAvailabilityModalComponent {
  nonAvailabilityForm: FormGroup;
  timeOptions: string[] = this.generateTimeOptions();
  constructor(
    private fb: FormBuilder,
    private nonAvailabilityService: NonAvailabilityService,
    public dialogRef: MatDialogRef<NonAvailabilityModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { accountId: number }
  ) {
    this.nonAvailabilityForm = this.fb.group({
      dateRange: this.fb.group({
        start: ['', Validators.required],
        end: ['', Validators.required],
      }),
      startTimeOnly: ['', Validators.required],
      endTimeOnly: ['', Validators.required],
    });
  }

  private convertToUTC(date: any, time: string): string {
    if (!date || !time) return '';

    let formattedDate: string;

    if (date instanceof Date) {
      const year = date.getFullYear();
      const month = ('0' + (date.getMonth() + 1)).slice(-2);
      const day = ('0' + date.getDate()).slice(-2);
      formattedDate = `${year}-${month}-${day}`;
    } else {
      formattedDate = date.split('/').reverse().join('-');
    }

    const combined = `${formattedDate}T${time}:00`;
    const parsed = new Date(combined);

    if (isNaN(parsed.getTime())) return '';

    parsed.setMinutes(parsed.getMinutes() - parsed.getTimezoneOffset());
    return parsed.toISOString();
  }

  onSave(): void {
    if (this.nonAvailabilityForm.invalid) return;

    const { dateRange, startTimeOnly, endTimeOnly } = this.nonAvailabilityForm.value;

    const payload: NonAvailability = {
      accountId: this.data.accountId,
      date1: this.convertToUTC(dateRange.start, startTimeOnly),
      date2: this.convertToUTC(dateRange.end, endTimeOnly),
    };

    this.nonAvailabilityService.createNonAvailability(payload).subscribe({
      next: () => {
        this.dialogRef.close(true); // ✅ Ensure we close with a success flag
      },
      error: (err) => {
        console.error('Error creating non-availability:', err);
        alert('Failed to save non-availability.');
      }
    });
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
  onCancel(): void {
    this.dialogRef.close(); // Will return `undefined`
  }
}
