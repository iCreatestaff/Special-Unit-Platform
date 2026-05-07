import { BreakpointObserver } from '@angular/cdk/layout';
import { Component, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { Subscription } from 'rxjs';
import { MatSidenav } from '@angular/material/sidenav';
import { RouterModule } from '@angular/router';
import { MaterialModule } from 'src/app/material.module';
import { CommonModule, NgFor } from '@angular/common';
import { SidebarComponent } from './sidebar/sidebar.component';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { TablerIconsModule } from 'angular-tabler-icons';
import { HeaderComponent } from './header/header.component';
import { AppNavItemComponent } from "./sidebar/nav-item/nav-item.component";
import { NavItem } from './sidebar/nav-item/nav-item';
import { NavService } from 'src/app/services/nav.service';

const MOBILE_VIEW = 'screen and (max-width: 768px)';
const TABLET_VIEW = 'screen and (min-width: 769px) and (max-width: 1024px)';
const MONITOR_VIEW = 'screen and (min-width: 1024px)';

@Component({
  selector: 'app-full',
  standalone: true,
  imports: [
    RouterModule,
    MaterialModule,
    CommonModule,
    SidebarComponent,
    NgScrollbarModule,
    TablerIconsModule,
    HeaderComponent,
    AppNavItemComponent,
    
  ],
  templateUrl: './full.component.html',
  styleUrls: ['./full.component.css'],  
  encapsulation: ViewEncapsulation.None,
  
})
export class FullComponent implements OnInit, OnDestroy {
  @ViewChild('leftsidenav') public sidenav!: MatSidenav;
  navItems: NavItem[] = [];
  
  private layoutChangesSubscription = Subscription.EMPTY;
  private isMobileScreen = false;
  private isContentWidthFixed = true;
  private isCollapsedWidthFixed = false;

  constructor(
    private breakpointObserver: BreakpointObserver,
    private navService: NavService
  ) {}

  ngOnInit(): void {
    this.navItems = this.navService.getFilteredNavItems();
    
    this.layoutChangesSubscription = this.breakpointObserver
      .observe([MOBILE_VIEW, TABLET_VIEW, MONITOR_VIEW])
      .subscribe((state) => {
        this.isMobileScreen = state.breakpoints[MOBILE_VIEW];
        this.isContentWidthFixed = state.breakpoints[MONITOR_VIEW];
      });
  }

  ngOnDestroy(): void {
    this.layoutChangesSubscription.unsubscribe();
  }

  get isOver(): boolean {
    return this.isMobileScreen;
  }

  toggleCollapsed(): void {
    this.isContentWidthFixed = false;
  }

  onSidenavClosedStart(): void {
    this.isContentWidthFixed = false;
  }

  onSidenavOpenedChange(isOpened: boolean): void {
    this.isCollapsedWidthFixed = !this.isOver;
  }
}