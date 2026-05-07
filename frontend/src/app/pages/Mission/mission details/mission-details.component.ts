import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { forkJoin } from 'rxjs';
import { MaterialModule } from 'src/app/material.module';
import { Account } from 'src/app/Models/account.model';
import { Equipment } from 'src/app/Models/equipment.model';
import { Mission } from 'src/app/Models/mission.model';
import { AccountService } from 'src/app/services/account.service';
import { EquipmentService } from 'src/app/services/equipment.service';
import { MissionService } from 'src/app/services/mission.service';
import * as L from 'leaflet';
import { Marker, icon } from 'leaflet';
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-mission-details',
  standalone: true,
  imports: [
    MatDialogModule,
    CommonModule,
    MaterialModule,
    MatTableModule
  ],
  templateUrl: './mission-details.component.html',
  styleUrls: ['./mission-details.component.css']
})
export class MissionDetailsComponent implements OnInit, AfterViewInit, OnDestroy {
  mission: Mission | null = null;
  admin: Account | null = null;
  assignedAccounts: Account[] = [];
  assignedEquipments: Equipment[] = [];
  isLoading = true;
  showInfoPanel = false;

  displayedColumnsAgents = ['photo', 'name', 'role'];
  displayedColumnsEquipments = ['photo', 'name', 'availability'];

  private map: L.Map | null = null;
  private mainMarker: L.Marker | null = null;
  private agentMarkers: L.Marker[] = [];

  constructor(
    public dialogRef: MatDialogRef<MissionDetailsComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { id: number },
    private missionService: MissionService,
    private accountService: AccountService,
    private equipmentService: EquipmentService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    if (this.data.id) {
      this.loadMissionDetails(this.data.id);
    } else {
      console.error('Invalid Mission ID');
    }
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      if (!this.map) {
        console.warn('Map was not initialized yet.');
      }
    }, 500);
  }

  loadMissionDetails(missionId: number): void {
    this.missionService.getMissionById(missionId).subscribe({
      next: (mission) => {
        this.mission = mission;

        const admin$ = this.accountService.getAccountById(mission.adminId);
        const accounts$ = mission.assignedAccounts.map(id => this.accountService.getAccountById(id));
        const equipments$ = mission.assignedEquipments.map(id => this.equipmentService.getEquipmentById(id));

        forkJoin([admin$, ...accounts$, ...equipments$]).subscribe({
          next: ([admin, ...rest]) => {
            this.admin = admin;
            this.assignedAccounts = rest.slice(0, accounts$.length) as Account[];
            this.assignedEquipments = rest.slice(accounts$.length) as Equipment[];
            this.isLoading = false;
            this.initMap(mission.location);
          },
          error: (error) => {
            console.error('Error loading mission details:', error);
            this.isLoading = false;
          }
        });
      },
      error: (error) => {
        console.error('Error loading mission:', error);
        this.isLoading = false;
      }
    });
  }

  initMap(location: string): void {
    if (!location) {
      console.error('No location provided.');
      return;
    }

    const [lat, lng] = location.split(',').map(Number);
    if (isNaN(lat) || isNaN(lng)) {
      console.error('Invalid location format.');
      return;
    }

    const mapContainer = document.getElementById('mission-map');
    if (!mapContainer) return;

    this.map = L.map('mission-map').setView([lat, lng], 13);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors'
    }).addTo(this.map);

    const customIcon = icon({
      iconUrl: 'assets/images/marker-icon.png',
      shadowUrl: 'assets/images/marker-shadow.png',
      iconSize: [30, 45],
      iconAnchor: [15, 45],
      popupAnchor: [1, -34]
    });

    this.mainMarker = L.marker([lat, lng], { icon: customIcon })
      .addTo(this.map)
      .bindPopup(`<b>Mission Location</b><br>${this.mission?.description ?? ''}`)
      .openPopup();

    this.addAgentMarkers();
  }

  addAgentMarkers(): void {
    if (!this.map) return;

    this.clearAgentMarkers();

    const agentIcon = icon({
      iconUrl: 'assets/images/agent-marker.png', // You can set a custom icon for agents
      iconSize: [20, 20],
      iconAnchor: [15, 45],
      popupAnchor: [1, -34]
    });

    const validAgents = this.assignedAccounts.filter(agent => 
      agent.latitude && agent.longitude &&
      !isNaN(+agent.latitude) && !isNaN(+agent.longitude)
    );

    validAgents.forEach(agent => {
      const marker = L.marker([+agent.latitude!, +agent.longitude!], { icon: agentIcon })
        .addTo(this.map!)
        .bindPopup(`
          <div class="agent-popup">
            <img src="${agent.photo}" onerror="this.src='assets/default-avatar.png'" class="popup-avatar" />
            <div class="popup-info">
              <h4>${agent.name}</h4>
              <p>${agent.role}</p>
            </div>
          </div>
        `);
      this.agentMarkers.push(marker);
    });

    if (this.mainMarker && this.agentMarkers.length) {
      const group = L.featureGroup([this.mainMarker, ...this.agentMarkers]);
      this.map.fitBounds(group.getBounds().pad(0.2));
    }
  }

  clearAgentMarkers(): void {
    this.agentMarkers.forEach(marker => marker.remove());
    this.agentMarkers = [];
  }

  ngOnDestroy(): void {
    this.map?.remove();
    this.mainMarker = null;
    this.agentMarkers = [];
  }

  toggleInfoPanel(): void {
    this.showInfoPanel = !this.showInfoPanel;
  }

  hideInfoPanel(): void {
    this.showInfoPanel = false;
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
