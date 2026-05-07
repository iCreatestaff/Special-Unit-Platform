import { Routes } from '@angular/router';
import { RequestMaintenanceListComponent } from './request-list/request-maintenance.component';


// Ensure correct path

export const RequestRoutes: Routes = [

  {
      path: '',
      children: [
        {
          path: 'request-list',
          component:RequestMaintenanceListComponent,
        },
      
        
        
      ],
    },
];
