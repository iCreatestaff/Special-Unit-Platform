import {
  Component,
  Output,
  EventEmitter,
  Input,
  ViewEncapsulation,
  ViewChild,
  ElementRef,
  HostListener,
  ChangeDetectorRef,
  OnDestroy
} from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { MatButtonModule } from '@angular/material/button';
import { NotificationService } from 'src/app/services/notification.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Notification } from 'src/app/Models/notification.model';
import { MaterialModule } from 'src/app/material.module';
import { forkJoin, map, Observable, Subject, takeUntil } from 'rxjs';
import { MatBadgeModule } from '@angular/material/badge';
import { MatMenuModule } from '@angular/material/menu';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    RouterModule,
    CommonModule,
    NgScrollbarModule,
    MatButtonModule,
    MaterialModule,
    MatBadgeModule,
    MatMenuModule
  ],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class HeaderComponent implements OnDestroy {
  notifications: Notification[] = [];
  unreadCount = 0;
  loading = true;
  error = '';
  showNotificationPanel = false;
  private destroy$ = new Subject<void>();

  @Input() showToggle = true;
  @Input() toggleChecked = false;
  @Output() toggleMobileNav = new EventEmitter<void>();
  @Output() toggleCollapsed = new EventEmitter<void>();
  
  @ViewChild('notificationPanel', { static: false }) 
  notificationPanelRef!: ElementRef;

  constructor(
    private notificationService: NotificationService,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadNotifications();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    const clickedInside = this.notificationPanelRef?.nativeElement?.contains(target);
    const isToggleBtn = target.closest('button[aria-label="Notifications"]');

    if (!clickedInside && !isToggleBtn && this.showNotificationPanel) {
      this.showNotificationPanel = false;
      this.cdr.detectChanges();
    }
  }

  logout(): void {
    localStorage.removeItem('accountId');
    localStorage.removeItem('role');
    localStorage.removeItem('type');
  }

  loadNotifications(): void {
    this.loading = true;
    this.error = '';
    const role = localStorage.getItem('role');
    const type = localStorage.getItem('type');
    
    let notificationObservable: Observable<Notification[]>;
    
    if (role === 'SuperAdmin' || type === 'maintenance') {
      notificationObservable = this.notificationService.getAllNotifications();
    } else {
      // For non-maintenance users, combine both mission and training notifications
      const missionNotifications$ = this.notificationService.getByTypeAsync('mission');
      const trainingNotifications$ = this.notificationService.getByTypeAsync('training');
      
      notificationObservable = forkJoin([missionNotifications$, trainingNotifications$]).pipe(
        takeUntil(this.destroy$),
        map(([missions, trainings]) => [...missions, ...trainings])
      );
    }

    notificationObservable.subscribe({
      next: (notifications) => {
        this.notifications = notifications
          .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
          .slice(0, 50); // Limit to 50 most recent
        
        this.unreadCount = this.notifications.filter(n => !n.isRead).length;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error fetching notifications:', err);
        this.error = 'Failed to load notifications. Please try again.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  toggleNotificationPanel(): void {
    this.showNotificationPanel = !this.showNotificationPanel;
    if (this.showNotificationPanel) {
      this.loadNotifications();
    }
    this.cdr.detectChanges();
  }

  markAsRead(notification: Notification, event?: Event): void {
    event?.stopPropagation();
    
    if (!notification.isRead) {
      this.notificationService.markAsRead(notification.id).pipe(
        takeUntil(this.destroy$)
      ).subscribe({
        next: () => {
          notification.isRead = true;
          this.unreadCount = Math.max(0, this.unreadCount - 1);
          this.showSnackbar('Marked as read');
          this.cdr.detectChanges();
        },
        error: () => {
          this.showSnackbar('Failed to mark as read');
        }
      });
    }
  }

  deleteNotification(notificationId: number, event: Event): void {
    event.stopPropagation();
    
    if (confirm('Are you sure you want to delete this notification?')) {
      const notification = this.notifications.find(n => n.id === notificationId);
      this.notificationService.deleteNotification(notificationId).pipe(
        takeUntil(this.destroy$)
      ).subscribe({
        next: () => {
          this.notifications = this.notifications.filter(n => n.id !== notificationId);
          if (notification && !notification.isRead) {
            this.unreadCount = Math.max(0, this.unreadCount - 1);
          }
          this.showSnackbar('Notification deleted');
          this.cdr.detectChanges();
        },
        error: () => {
          this.showSnackbar('Failed to delete notification');
        }
      });
    }
  }

  trackById(index: number, item: Notification): number {
    return item.id;
  }

  private showSnackbar(message: string): void {
    this.snackBar.open(message, 'Close', { 
      duration: 3000,
      panelClass: ['snackbar-notification']
    });
  }
}