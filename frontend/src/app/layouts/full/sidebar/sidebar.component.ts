import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { BrandingComponent } from './branding.component';
import { TablerIconsModule } from 'angular-tabler-icons';
import { MaterialModule } from 'src/app/material.module';
import { NavItem } from './nav-item/nav-item';
import { RouterModule } from '@angular/router';

import { CommonModule } from '@angular/common';
import { NavService } from 'src/app/services/nav.service';
import { AppNavItemComponent } from "./nav-item/nav-item.component";

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [BrandingComponent, TablerIconsModule, MaterialModule, RouterModule, CommonModule, AppNavItemComponent],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
})
export class SidebarComponent implements OnInit {
  navItems: NavItem[] = [];
  constructor(private navigationService: NavService) {
   
   }
  @Input() showToggle = true;
  @Output() toggleMobileNav = new EventEmitter<void>();
  @Output() toggleCollapsed = new EventEmitter<void>();

  ngOnInit() {
    this.navItems = this.navigationService.getFilteredNavItems();
  }

}