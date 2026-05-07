import { Component, OnDestroy, OnInit, Inject, ViewEncapsulation, CUSTOM_ELEMENTS_SCHEMA, ChangeDetectorRef } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { Mission } from 'src/app/Models/mission.model';
import { Account } from 'src/app/Models/account.model';
import { Equipment } from 'src/app/Models/equipment.model';
import { MissionService } from 'src/app/services/mission.service';
import { AccountService } from 'src/app/services/account.service';
import { EquipmentService } from 'src/app/services/equipment.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE, MatNativeDateModule, MatOptionModule, NativeDateAdapter } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MaterialModule } from 'src/app/material.module';
import { MatButtonModule } from '@angular/material/button';
import { LocationPickerDialogComponent } from '../location-picker/location-picker.component';

import { LeafletModule } from '@asymmetrik/ngx-leaflet';

@Component({
    selector: 'app-mission-form',
    standalone:true,
    imports: [
        CommonModule,
        ReactiveFormsModule, FormsModule,
        MatCheckboxModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatFormFieldModule,
        MatOptionModule,
        MatSelectModule,
        MaterialModule,
        MatStepperModule,
        MatButtonModule,
        MaterialModule,LeafletModule
    ],
    providers: [
        { provide: MAT_DATE_LOCALE, useValue: 'en-GB' },
        { provide: DateAdapter, useClass: NativeDateAdapter },
        {
            provide: MAT_DATE_FORMATS,
            useValue: {
                parse: { dateInput: 'YYYY-MM-DDTHH:mm' },
                display: { dateInput: 'YYYY-MM-DDTHH:mm' },
            },
        },
    ],
    templateUrl: './mission-form.component.html',
    styleUrls: ['./mission-form.component.css'],
    encapsulation: ViewEncapsulation.None,
    schemas: [CUSTOM_ELEMENTS_SCHEMA]

})
export class MissionFormComponent implements OnInit, OnDestroy {
  missionForm: FormGroup;
  isLoading = false;
  admins: Account[] = [];
  agents: Account[] = [];
  equipments: Equipment[] = [];
  selectedLocation: { lat: number; lng: number } | null = null;
  private isDestroyed = false;

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    public dialogRef: MatDialogRef<MissionFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { id?: number } | null,
    private missionService: MissionService,
    private accountService: AccountService,
    private equipmentService: EquipmentService,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef
  ) {
    this.missionForm = this.createForm();
  }

  ngOnInit(): void {
    if (this.data?.id) {
      this.fetchMissionDetails();
    }
  }

  ngOnDestroy(): void {
    this.isDestroyed = true;
  }

  private createForm(): FormGroup {
    return this.fb.group({
      basicInfo: this.fb.group({
        description: ['', Validators.required],
        location: ['', Validators.required],
        type: ['', Validators.required],
        dateRange: this.fb.group({
          start: [null, Validators.required],
          end: [null, Validators.required]
        }),
        startTime: ['', Validators.required],
        endTime: ['', Validators.required]
      }),
      resources: this.fb.group({
       
        assignedAccounts: [[], [Validators.required, Validators.minLength(1)]],
        assignedEquipments: [[], [Validators.required, Validators.minLength(1)]]
      })
    });
  }

  get basicInfo(): FormGroup {
    return this.missionForm.get('basicInfo') as FormGroup;
  }

  get resources(): FormGroup {
    return this.missionForm.get('resources') as FormGroup;
  }

  openLocationModal(): void {
    const dialogRef = this.dialog.open(LocationPickerDialogComponent, {
      width: '100%',
      maxWidth: '1000px',
      height: '60%',
      maxHeight: '80vh',
      panelClass: 'location-picker-dialog',
      data: {
        editLocation: this.basicInfo.get('location')?.value
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const location = `${result.latitude},${result.longitude}`;
        this.basicInfo.get('location')?.setValue(location);
        this.selectedLocation = { lat: result.latitude, lng: result.longitude };
        this.refreshView();
      }
    });
  }

  clearLocation(): void {
    this.basicInfo.get('location')?.setValue('');
    this.selectedLocation = null;
    this.refreshView();
  }

  loadDropdowns(): void {
    const startTime = this.combineDateTime(
      this.basicInfo.get('dateRange.start')?.value,
      this.basicInfo.get('startTime')?.value
    );
    const endTime = this.combineDateTime(
      this.basicInfo.get('dateRange.end')?.value,
      this.basicInfo.get('endTime')?.value
    );

    if (!startTime || !endTime) return;

    if (this.data?.id) {
      this.missionService.deleteNonavailabilities(this.data.id).subscribe({
        next: () => this.fetchAvailableResources(startTime, endTime),
        error: (error) => this.handleError('Failed to delete non-availability data', error)
      });
    } else {
      this.fetchAvailableResources(startTime, endTime);
    }
  }

  private fetchAvailableResources(startTime: string, endTime: string): void {
    this.accountService.getAvailableAccounts(startTime, endTime).subscribe({
      next: (accounts) => {
        this.admins = accounts.filter(a => a.role === 'Admin');
        this.agents = accounts.filter(a => a.role === 'Agent');
        this.refreshView();
      },
      error: (error) => this.handleError('Error fetching accounts', error)
    });

    this.equipmentService.getAvailableEquipments(startTime, endTime).subscribe({
      next: (equipments) => {
        this.equipments = equipments;
        this.refreshView();
      },
      error: (error) => this.handleError('Error fetching equipment', error)
    });
  }

  fetchMissionDetails(): void {
    if (!this.data?.id) return;
    
    this.isLoading = true;
    this.missionService.getMissionById(this.data.id).subscribe({
      next: (mission) => {
        this.patchFormValues(mission);
        this.isLoading = false;
        this.loadDropdowns();
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.showError('Failed to load mission details');
        this.refreshView();
      }
    });
  }

  private patchFormValues(mission: Mission): void {
    const startDate = new Date(mission.startTime);
    const endDate = new Date(mission.endTime);

    this.basicInfo.patchValue({
      description: mission.description,
      location: mission.location,
      type: mission.type,
      dateRange: {
        start: startDate,
        end: endDate
      },
      startTime: this.formatTime(startDate),
      endTime: this.formatTime(endDate)
    });

    this.resources.patchValue({
      adminId: mission.adminId,
      assignedAccounts: mission.assignedAccounts,
      assignedEquipments: mission.assignedEquipments
    });

    if (mission.location) {
      const [lat, lng] = mission.location.split(',').map(Number);
      if (!isNaN(lat) && !isNaN(lng)) {
        this.selectedLocation = { lat, lng };
      }
    }
  }

  onSubmit(): void {
    if (this.missionForm.invalid) {
      this.markFormGroupTouched(this.missionForm);
      this.showError('Please fill all required fields');
      return;
    }

    const missionData = this.prepareMissionData();
    this.isLoading = true;

    const operation = this.data?.id
      ? this.missionService.updateMission(this.data.id, missionData)
      : this.missionService.createMission(missionData);

    operation.subscribe({
      next: () => {
        this.showSuccess(`Mission ${this.data?.id ? 'updated' : 'created'} successfully`);
        this.dialogRef.close(true);
      },
      error: (error) => {
        this.isLoading = false;
        this.handleError(`Error ${this.data?.id ? 'updating' : 'creating'} mission`, error);
        this.refreshView();
      }
    });
  }

  private prepareMissionData(): Mission {
    const storedAccount = localStorage.getItem('accountId');
    const parsedAccount = storedAccount ? JSON.parse(storedAccount) : null;
    const formValue = this.missionForm.value;
    return {
      id: this.data?.id,
      description: formValue.basicInfo.description,
      location: formValue.basicInfo.location,
      type: formValue.basicInfo.type,
      startTime: this.combineDateTime(
        formValue.basicInfo.dateRange.start,
        formValue.basicInfo.startTime
      ),
      endTime: this.combineDateTime(
        formValue.basicInfo.dateRange.end,
        formValue.basicInfo.endTime
      ),
      adminId: parsedAccount,
      assignedAccounts: formValue.resources.assignedAccounts,
      assignedEquipments: formValue.resources.assignedEquipments
    };
  }

  nextStep(stepper: MatStepper): void {
    if (this.basicInfo.invalid) {
      this.markFormGroupTouched(this.basicInfo);
      this.showError('Please complete all required fields');
      return;
    }

    this.loadDropdowns();
    stepper.next();
  }

  previousStep(stepper: MatStepper): void {
    stepper.previous();
  }

  cancel(): void {
    this.dialogRef.close();
  }

  // Helper methods
  private formatTime(date: Date): string {
    return date.toTimeString().slice(0, 5);
  }

  private combineDateTime(date: Date | null, time: string): string {
    if (!date || !time) return '';
    const [hours, minutes] = time.split(':').map(Number);
    const localDate = new Date(date);
    localDate.setHours(hours, minutes, 0, 0);
  
    // Format to "YYYY-MM-DDTHH:mm" in local time
    const year = localDate.getFullYear();
    const month = String(localDate.getMonth() + 1).padStart(2, '0');
    const day = String(localDate.getDate()).padStart(2, '0');
    const hour = String(localDate.getHours()).padStart(2, '0');
    const minute = String(localDate.getMinutes()).padStart(2, '0');
  
    return `${year}-${month}-${day}T${hour}:${minute}`;
  }
  

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  private showSuccess(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      panelClass: ['success-snackbar']
    });
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      panelClass: ['error-snackbar']
    });
  }

  private handleError(message: string, error: any): void {
    console.error(`${message}:`, error);
    this.showError(message);
    this.refreshView();
  }

  private refreshView(): void {
    if (!this.isDestroyed) {
      this.cdr.detectChanges();
    }
  }
}
