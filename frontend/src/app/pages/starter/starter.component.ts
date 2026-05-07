import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { MaterialModule } from '../../material.module';
import { AppNewCustomersComponent } from 'src/app/components/new-customers/new-customers.component';
import { AppTotalIncomeComponent } from 'src/app/components/total-income/total-income.component';
import { AppDailyActivitiesComponent } from 'src/app/components/daily-activities/daily-activities.component';
import { AppBlogCardsComponent } from 'src/app/components/blog-card/blog-card.component';
import { AppRevenueProductComponent } from 'src/app/components/revenue-product/revenue-product.component';
import { AppRevenueForecastComponent } from 'src/app/components/revenue-forecast/revenue-forecast.component';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { isMaintenanceAgent } from 'src/app/layouts/full/sidebar/sidebar-data';
import { CommonModule } from '@angular/common';
import { routes } from 'src/app/app.routes';
import { filter } from 'rxjs';

@Component({
  selector: 'app-starter',
  standalone: true,
  imports: [
    MaterialModule,
    RouterModule,
    CommonModule
  ],
  templateUrl: './starter.component.html',
  styleUrls: ['./starter.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class StarterComponent implements OnInit {
  isMaintenanceAgent = false;
  isMenuOpen = false;

  constructor(private router: Router) {}

  ngOnInit() {
    const role = localStorage.getItem('role')?.trim();
    const type = localStorage.getItem('type');
    this.isMaintenanceAgent = role === 'Agent' && type === 'maintenance';
  }

  scrollTo(fragment: string) {
    const element = document.getElementById(fragment);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth' });
      this.router.navigate([], { fragment: fragment });
    }
    this.isMenuOpen = false; // Close menu on mobile after navigation
  }

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
    const nav = document.getElementById('mainNav');
    if (nav) {
      nav.style.display = this.isMenuOpen ? 'flex' : 'none';
    }
  }
}
