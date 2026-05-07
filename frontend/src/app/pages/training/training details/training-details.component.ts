import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  Inject,
  OnDestroy,
  OnInit
} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Training } from 'src/app/Models/training.model';
import { Account } from 'src/app/Models/account.model';
import { AccountService } from 'src/app/services/account.service';
import { forkJoin, of } from 'rxjs';
import { MaterialModule } from 'src/app/material.module';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { TrainingService } from 'src/app/services/training.service';
import * as L from 'leaflet';

@Component({
  selector: 'app-training-details',
  standalone: true,
  imports: [MaterialModule, CommonModule, MatTableModule],
  templateUrl: './training-details.component.html',
  styleUrls: ['./training-details.component.css']
})
export class TrainingDetailsComponent implements OnInit, AfterViewInit, OnDestroy {
  training: Training | null = null;
  assignedAccounts: Account[] = [];
  isLoading = true;
  showInfoPanel = false;

  private map: L.Map | null = null;
  private mainMarker: L.Marker | null = null;

  constructor(
    public dialogRef: MatDialogRef<TrainingDetailsComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { id: number },
    private accountService: AccountService,
    private trainingService: TrainingService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    if (this.data.id) {
      this.loadTrainingDetails(this.data.id);
    } else {
      console.error('Invalid Training ID');
    }
  }

  ngAfterViewInit(): void {
    // Ensures map initializes after view is ready
    setTimeout(() => {
      if (this.training?.location) {
        this.initMap(this.training.location);
      }
    }, 500);
  }

  ngOnDestroy(): void {
    this.map?.remove();
    this.mainMarker = null;
  }

  loadTrainingDetails(trainingId: number): void {
    this.trainingService.getById(trainingId).subscribe({
      next: (training) => {
        this.training = training;

        const accounts$ = training.assignedAccounts.map(id =>
          this.accountService.getAccountById(id)
        );

        (accounts$.length ? forkJoin(accounts$) : of([] as Account[])).subscribe({
          next: (accounts) => {
            this.assignedAccounts = accounts;
            this.isLoading = false;
            this.cdr.detectChanges(); // Ensure change detection for map
            this.initMap(training.location);
          },
          error: (error) => {
            console.error('Error loading accounts:', error);
            this.isLoading = false;
          }
        });
      },
      error: (error) => {
        console.error('Error loading training:', error);
        this.isLoading = false;
      }
    });
  }

  initMap(location: string): void {
    if (!location) return;

    const [lat, lng] = location.split(',').map(Number);
    if (isNaN(lat) || isNaN(lng)) {
      console.error('Invalid location format');
      return;
    }

    const mapElement = document.getElementById('training-map');
    if (!mapElement) {
      console.error('Map element not found');
      return;
    }

    this.map = L.map('training-map').setView([lat, lng], 13);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors'
    }).addTo(this.map);

    const customIcon = L.icon({
      iconUrl: 'assets/images/marker-icon.png',
      shadowUrl: 'assets/images/marker-shadow.png',
      iconSize: [30, 45],
      iconAnchor: [15, 45],
      popupAnchor: [0, -34]
    });

    this.mainMarker = L.marker([lat, lng], { icon: customIcon })
      .addTo(this.map)
     
      .openPopup();
  }

  toggleInfoPanel(): void {
    this.showInfoPanel = !this.showInfoPanel;
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
