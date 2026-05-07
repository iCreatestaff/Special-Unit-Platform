import { Routes } from '@angular/router';
import { BlankComponent } from './layouts/blank/blank.component';
import { FullComponent } from './layouts/full/full.component';
import { RedirectComponent } from './redirect.component';
import { AuthGuard } from './auth.guard';
; // Import redirect component

export const routes: Routes = [
  {
    path: '',
    component: BlankComponent,
    children: [
      {
        path: '',
        component: RedirectComponent, // ✅ redirect logic here
      },
      {
        path: 'authentication',
        loadChildren: () =>
          import('./pages/authentication/authentication.routes').then(
            (m) => m.AuthenticationRoutes
          ),
      },
      {
        path: 'dashboard',
        loadChildren: () =>
          import('./pages/pages.routes').then((m) => m.PagesRoutes),
        canActivate: [AuthGuard],
      },
    ],
  },
  {
    path: '',
    component: FullComponent,
    children: [
      {
        path: '',
        redirectTo: '/dashboard',
        pathMatch: 'full',
      },
     
      {
        path: 'profile',
        loadChildren: () =>
          import('./pages/pages1.route').then((m) => m.Pages1Routes),
        canActivate: [AuthGuard],
      },
      
      {
        path: 'ui-components',
        loadChildren: () =>
          import('./pages/ui-components/ui-components.routes').then(
            (m) => m.UiComponentsRoutes
          ),
        canActivate: [AuthGuard],
      },
      {
        path: 'extra',
        loadChildren: () =>
          import('./pages/extra/extra.routes').then(
            (m) => m.ExtraRoutes
          ),
        canActivate: [AuthGuard],
      },
      {
        path: 'Equipment',
        loadChildren: () =>
          import('./pages/Equipment/equipment.routes').then(
            (m) => m.EquipmentRoutes
          ),
        canActivate: [AuthGuard],
        
      },
      {
        path: 'account',
        loadChildren: () =>
          import('./pages/account/account.routes').then(
            (m) => m.AccountRoutes
          ),
        canActivate: [AuthGuard],
      },
      {
        path: 'Mission',
        loadChildren: () =>
          import('./pages/Mission/mission.routes').then(
            (m) => m.MissionRoutes
          ),
        canActivate: [AuthGuard],
      },
      {
        path: 'SubEquipment',
        loadChildren: () =>
          import('./pages/SubEquipment/subEquipment.routes').then(
            (m) => m.SubEquipmentRoutes
          ),
        canActivate: [AuthGuard],
      },
      {
        path: 'EquipmentStock',
        loadChildren: () =>
          import('./pages/EquipmentStock/equipmentStock.routes').then(
            (m) => m.EquipmentStockRoutes
          ),
        canActivate: [AuthGuard],
      },
      {
        path: 'Maintenance',
        loadChildren: () =>
          import('./pages/Maintenance/maintenance.routes').then(
            (m) => m.MaintenanceRoutes
          ),
        canActivate: [AuthGuard],
      },
      {
        path: 'training',
        loadChildren: () =>
          import('./pages/training/training.routes').then(
            (m) => m.TrainingRoutes
          ),
        canActivate: [AuthGuard],
      },
      {
        path: 'request-maintenance',
        loadChildren: () =>
          import('./pages/request-maintenance/request.routes').then(
            (m) => m.RequestRoutes
          ),
        canActivate: [AuthGuard],
      },
    ],
  },
 
];
