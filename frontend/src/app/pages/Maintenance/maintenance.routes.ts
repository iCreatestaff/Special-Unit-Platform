import { Routes } from '@angular/router';
import { MaintenanceComponent } from './maintenance list/maintenance.component';


// Ensure correct path

export const MaintenanceRoutes: Routes = [

  {
      path: '',
      children: [
        {
          path: 'maintenance-list',
          component:MaintenanceComponent,
        },
      
        
        
      ],
    },
];
