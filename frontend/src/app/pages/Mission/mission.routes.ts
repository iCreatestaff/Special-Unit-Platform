import { Routes } from '@angular/router';
import { MissionComponent } from './mission list/mission.component';

// Ensure correct path

export const MissionRoutes: Routes = [

  {
      path: '',
      children: [
        {
          path: 'mission-list',
          component:MissionComponent,
        },
      
        
        
      ],
    },
];
