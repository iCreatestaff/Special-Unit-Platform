import { ChangeDetectionStrategy, ChangeDetectorRef, Component, EventEmitter, Inject, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { EquipmentStockService } from 'src/app/services/equipment-stock.service';
import { Equipment } from 'src/app/Models/equipment.model';
import { MaterialModule } from 'src/app/material.module';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatStepperModule } from '@angular/material/stepper';
import { EquipmentService } from 'src/app/services/equipment.service';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { SubEquipment } from 'src/app/Models/sub-equipment.model';
import { EquipmentStock } from 'src/app/Models/equipment-stock.model';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-update-all-equipment',
  standalone: true,
  imports: [
    MaterialModule,
    MatButtonModule,
    ReactiveFormsModule,
    CommonModule,
    MatTableModule,
    MatStepperModule,
    MatInputModule,
    MatSelectModule,
    MatFormFieldModule,
    FormsModule
  ],
  templateUrl: './update-all-equipment.component.html',
  styleUrls: ['./update-all-equipment.component.css'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UpdateAllEquipmentComponent implements OnInit {
  @Output() refreshParent = new EventEmitter<void>();
  
  equipmentForm: FormGroup;
  subEquipmentForm: FormGroup;
  equipment: Equipment;
  fileName: string | null = null;
  equipments: Equipment[] = [];
  dataSource = new MatTableDataSource<Equipment>();
  isLoading = false;
  equipmentStock: EquipmentStock[] = [];
  isAddingSubEquipment = false;
  
  availableStatuses = [
    { value: 'bon_etat', label: 'Bon état' },
    { value: 'avant_panne', label: 'Avant panne' },
    { value: 'en_panne', label: 'En panne' }
  ];

  constructor(
    private fb: FormBuilder,
    private equipmentStockService: EquipmentStockService,
    private equipmentService: EquipmentService,
    public dialogRef: MatDialogRef<UpdateAllEquipmentComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { equipment: Equipment },
    private cdr: ChangeDetectorRef,private snackBar: MatSnackBar
  ) {
    this.equipment = data.equipment;
    this.equipmentForm = this.fb.group({
      name: ['', Validators.required],
      type: ['', Validators.required],
      availability: ['', Validators.required],
      photo: ['', Validators.required],
      equipmentStockId: ['', Validators.required],
    });
    
    this.subEquipmentForm = this.fb.group({
      subEquipments: this.fb.array([]),
    });
  }

  ngOnInit(): void {
    this.initForms();
    this.getEquipmentDetails();
    this.loadStock();
  }

  get subEquipments(): FormArray {
    return this.subEquipmentForm.get('subEquipments') as FormArray;
  }

  initForms(): void {
    this.equipmentForm.patchValue({
      name: this.equipment?.name || '',
      type: this.equipment?.type || '',
      availability: this.equipment?.availability || '',
      photo: this.equipment?.photo || '',
      equipmentStockId: this.equipment?.equipmentStockId || ''
    });

    if (this.equipment?.subEquipments) {
      this.loadSubEquipments(this.equipment.subEquipments);
    }
  }

  loadStock(): void {
    this.isLoading = true;
    this.equipmentStockService.fetchStock().subscribe({
      next: (data) => {
        this.equipmentStock = data;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Error fetching stock:', err);
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  getEquipmentDetails(): void {
    if (!this.equipment?.id) return;
    
    this.isLoading = true;
    this.equipmentService.getEquipmentById(this.equipment.id).subscribe({
      next: (equipment: Equipment) => {
        this.equipment = equipment;
        this.equipmentForm.patchValue(equipment);
        this.loadSubEquipments(equipment.subEquipments || []);
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Error fetching equipment details:', err);
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  loadSubEquipments(subEquipments: SubEquipment[]): void {
    this.subEquipments.clear();
    subEquipments.forEach((subEq) => {
      const { cycleNumber, cycleUnit } = this.separateCycle(subEq.cycle);
      this.subEquipments.push(this.fb.group({
        name: [subEq.name, Validators.required],
        cycleNumber: [cycleNumber, [Validators.required, Validators.min(1)]],
        cycleUnit: [cycleUnit, Validators.required],
        status: [subEq.status || 'bon_etat', Validators.required]
      }));
    });
  }

  separateCycle(cycle: string): { cycleNumber: number, cycleUnit: string } {
    if (!cycle) return { cycleNumber: 1, cycleUnit: 'year' };
    const parts = cycle.split(' ');
    return parts.length === 2 
      ? { cycleNumber: Number(parts[0]), cycleUnit: parts[1] } 
      : { cycleNumber: 1, cycleUnit: 'year' };
  }

  addSubEquipment(): void {
    this.isAddingSubEquipment = true;
    this.subEquipments.push(this.fb.group({
      name: ['', Validators.required],
      cycleNumber: [1, [Validators.required, Validators.min(1)]],
      cycleUnit: ['year', Validators.required],
      status: ['bon_etat', Validators.required]
    }));
    this.cdr.markForCheck();
  }

removeSubEquipment(index: number): void {
  const subEquipmentName = this.subEquipments.at(index).get('name')?.value;
  
  if (!subEquipmentName) {
    this.subEquipments.removeAt(index);
    this.cdr.markForCheck();
    return;
  }

  if (confirm(`Are you sure you want to delete the sub-equipment "${subEquipmentName}" from all equipment?`)) {
    this.isLoading = true;
    
    this.equipmentStockService.deleteSubEquipmentFromAllEquipments(
      this.equipment.equipmentStockId!,
      subEquipmentName
    ).subscribe({
      next: (response) => {
        if (response.success) {
          this.subEquipments.removeAt(index);
          this.snackBar.open(response.message || 'Sub-equipment deleted successfully', 'Close', {
            duration: 3000
          });
        } else {
          this.snackBar.open(response.message || 'Failed to delete sub-equipment', 'Close', {
            duration: 3000,
            panelClass: ['error-snackbar']
          });
        }
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Error deleting sub-equipment:', err);
        this.snackBar.open('Error communicating with server', 'Close', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }
}
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      const file = input.files[0];
      this.fileName = file.name;
      
      const reader = new FileReader();
      reader.onload = (e: ProgressEvent<FileReader>) => {
        const img = new Image();
        img.onload = () => {
          const canvas = document.createElement('canvas');
          const ctx = canvas.getContext('2d');
          if (!ctx) return;
          
          const MAX_SIZE = 800;
          let width = img.width;
          let height = img.height;
          
          if (width > height && width > MAX_SIZE) {
            height *= MAX_SIZE / width;
            width = MAX_SIZE;
          } else if (height > MAX_SIZE) {
            width *= MAX_SIZE / height;
            height = MAX_SIZE;
          }
          
          canvas.width = width;
          canvas.height = height;
          ctx.drawImage(img, 0, 0, width, height);
          
          this.equipmentForm.patchValue({
            photo: canvas.toDataURL('image/jpeg', 0.7)
          });
          this.cdr.markForCheck();
        };
        img.src = e.target?.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  submit(): void {
    if (this.equipmentForm.invalid || this.subEquipmentForm.invalid) {
      this.markAllAsTouched(this.equipmentForm);
      this.markAllAsTouched(this.subEquipmentForm);
      return;
    }

    const subEquipments = this.subEquipments.controls.map(control => ({
      name: control.get('name')?.value,
      cycle: `${control.get('cycleNumber')?.value} ${control.get('cycleUnit')?.value}`,
      status: control.get('status')?.value
    }));

    const payload = {
      ...this.equipmentForm.value,
      subEquipments
    };

    this.isLoading = true;
    this.equipmentStockService.updateAllEquipmentStock(
      this.equipment.equipmentStockId!, 
      payload
    ).subscribe({
      next: () => {
        this.refreshParent.emit();
        this.dialogRef.close(true);
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error updating equipment:', err);
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  private markAllAsTouched(form: FormGroup | FormArray): void {
    Object.values(form.controls).forEach(control => {
      control.markAsTouched();
      if (control instanceof FormGroup || control instanceof FormArray) {
        this.markAllAsTouched(control);
      }
    });
  }
}