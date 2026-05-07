import { ChangeDetectorRef, Component, EventEmitter, Output, OnDestroy, OnInit, Inject, ViewChild, ElementRef } from '@angular/core';
import Fuse from 'fuse.js';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import * as L from 'leaflet';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { Map as LeafletMap } from 'leaflet';

import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { Subject, debounceTime, distinctUntilChanged, switchMap, catchError, of, Observable } from 'rxjs';

interface CitySuggestion {
  name: string;
  lat: number;
  lon: number;
  display_name?: string;
}

@Component({
  selector: 'app-location-picker',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    FormsModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatListModule
  ],
  templateUrl: './location-picker.component.html',
  styleUrls: ['./location-picker.component.css'],
})
export class LocationPickerDialogComponent implements OnInit, OnDestroy {
  latitude: number = 0;
  longitude: number = 0;
  map: LeafletMap;
  currentLocationMarker: L.Marker;
  accuracyCircle: L.Circle | null = null;
  coverageCircle: L.Circle | null = null;
  coverageRadius: number = 1000; // in meters
  searchQuery: string = '';
  citySuggestions: CitySuggestion[] = [];
  isLoading: boolean = false;
  fuse: any;
  searchTerm$ = new Subject<string>();
  searchCache: globalThis.Map<string, CitySuggestion[]> = new Map();
  private isDestroyed = false;
  
  
  
  @Output() locationSelected = new EventEmitter<{ latitude: number; longitude: number }>();
  editLocation: string | null = null;
  @ViewChild('searchInput') searchInput: ElementRef;

  constructor(
    public dialogRef: MatDialogRef<LocationPickerDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private cdr: ChangeDetectorRef
  ) {
    this.editLocation = data?.editLocation || null;
  }

  ngOnInit(): void {
    this.initializeMap();
    this.initializeFuse();
    this.setupSearchListener();
    
    if (this.editLocation) {
      const [lat, lng] = this.editLocation.split(',').map(Number);
      if (!isNaN(lat) && !isNaN(lng)) {
        this.setLocation(lat, lng, 13);
        return;
      }
    }
    
    this.getCurrentLocation();
  }

  private initializeMap(): void {
    const defaultCenter: L.LatLngTuple = [51.505, -0.09];
    this.map = L.map('map').setView(defaultCenter, 13);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors',
      maxZoom: 18,
    }).addTo(this.map);

    const customIcon = L.icon({
      iconUrl: 'assets/images/marker-icon.png',
      shadowUrl: 'assets/images/marker-shadow.png',
      iconSize: [25, 41],
      iconAnchor: [12, 41],
      popupAnchor: [1, -34],
      shadowSize: [41, 41]
    });

    this.currentLocationMarker = L.marker(defaultCenter, {
      icon: customIcon,
      draggable: true
    }).addTo(this.map);

    this.currentLocationMarker.on('dragend', () => {
      const position = this.currentLocationMarker.getLatLng();
      this.setLocation(position.lat, position.lng);
    });

    this.map.on('click', (e: L.LeafletMouseEvent) => {
      this.setLocation(e.latlng.lat, e.latlng.lng);
    });
  }

  private setupSearchListener(): void {
    this.searchTerm$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap((query: string) => {
        if (!query.trim()) {
          return of([]);
        }
        return this.fetchCitySuggestions(query);
      }),
      catchError(error => {
        console.error('Search error:', error);
        return of([]);
      })
    ).subscribe((results: CitySuggestion[]) => {
      this.citySuggestions = results;
      this.isLoading = false;
      this.refreshView();
    });
  }

  ngOnDestroy(): void {
    this.isDestroyed = true;
  }

  private initializeFuse(): void {
    this.fuse = new Fuse([], {
      keys: ['name', 'display_name'],
      includeScore: true,
      threshold: 0.4,
      minMatchCharLength: 2,
    });
  }

  searchCity(): void {
    const query = this.searchQuery.trim();
    if (!query) {
      this.citySuggestions = [];
      this.refreshView();
      return;
    }

    this.isLoading = true;
    this.refreshView();
    this.searchTerm$.next(query);
  }

  private fetchCitySuggestions(query: string): Observable<CitySuggestion[]> {
    // Check cache first
    if (this.searchCache.has(query)) {
      return of(this.searchCache.get(query)!);
    }

    return new Observable(observer => {
      fetch(`https://nominatim.openstreetmap.org/search?q=${encodeURIComponent(query)}&format=json&addressdetails=1&limit=8`)
        .then(response => {
          if (!response.ok) throw new Error('Network response was not ok');
          return response.json();
        })
        .then((data: any[]) => {
          const results = data.map(item => ({
            name: item.display_name,
            lat: parseFloat(item.lat),
            lon: parseFloat(item.lon),
            display_name: item.display_name
          }));
          
          // Update cache
          this.searchCache.set(query, results);
          
          // Update Fuse.js collection
          this.fuse.setCollection(results);
          
          observer.next(results);
          observer.complete();
        })
        .catch(error => {
          console.error('Error fetching city suggestions:', error);
          observer.next([]);
          observer.complete();
        });
    });
  }

  selectCity(location: CitySuggestion): void {
    this.setLocation(location.lat, location.lon, 15);
    this.searchQuery = location.name;
    this.citySuggestions = [];
    this.refreshView();
  }

  private setLocation(lat: number, lng: number, zoom?: number): void {
    this.latitude = lat;
    this.longitude = lng;
    this.updateCurrentLocationMarker([lat, lng]);
    if (zoom) {
      this.map.setView([lat, lng], zoom);
    }

    if (this.accuracyCircle) {
      this.map.removeLayer(this.accuracyCircle);
      this.accuracyCircle = null;
    }
    if (this.coverageCircle) {
      this.map.removeLayer(this.coverageCircle);
    }
    this.coverageCircle = L.circle([lat, lng], {
      radius: 1000, // Coverage radius in meters
      color: '#1976d2',
      fillColor: '#2196f3',
      fillOpacity: 0.15,
      weight: 2,
    }).addTo(this.map);
    this.refreshView();
  }

  updateCurrentLocationMarker(latlng: L.LatLngExpression): void {
    this.currentLocationMarker.setLatLng(latlng);
  }

  getCurrentLocation(): void {
    if (!navigator.geolocation) {
      this.useDefaultLocation();
      return;
    }

    this.isLoading = true;
    this.refreshView();

    navigator.geolocation.getCurrentPosition(
      position => {
        const { latitude, longitude, accuracy } = position.coords;

        this.setLocation(latitude, longitude, 13);

        if (accuracy > 100) {
          alert(`Location accuracy is low (±${Math.round(accuracy)}m). Please verify or adjust the pin.`);
        }

        this.accuracyCircle = L.circle([latitude, longitude], {
          radius: accuracy,
          color: 'blue',
          fillColor: '#99ccff',
          fillOpacity: 0.2
        }).addTo(this.map);

        this.isLoading = false;
        this.refreshView();
      },
      error => {
        console.error('Geolocation error:', error);
        alert('Could not get current location. Please select manually.');
        this.useDefaultLocation();
        this.isLoading = false;
        this.refreshView();
      },
      { enableHighAccuracy: true, timeout: 10000, maximumAge: 0 }
    );
  }

  private useDefaultLocation(): void {
    this.setLocation(51.505, -0.09, 13);
  }

  selectLocation(): void {
    if (!this.latitude || !this.longitude) return;
    this.locationSelected.emit({ latitude: this.latitude, longitude: this.longitude });
    this.dialogRef.close({ latitude: this.latitude, longitude: this.longitude });
  }

  clearSearch(): void {
    this.searchQuery = '';
    this.citySuggestions = [];
    this.searchInput.nativeElement.focus();
    this.refreshView();
  }

  closeDialog(): void {
    this.dialogRef.close();
  }

  updateCoverage(): void {
    this.setLocation(this.latitude, this.longitude);
  }

  private refreshView(): void {
    if (!this.isDestroyed) {
      this.cdr.detectChanges();
    }
  }
}
