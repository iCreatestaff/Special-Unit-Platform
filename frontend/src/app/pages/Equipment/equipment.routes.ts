import { Routes } from '@angular/router';
import { EquipmentDetailsComponent } from './equiment details/equipment-details';
import { EquipmentComponent } from './equipment list/equipment-list.component';
import { EquipmentModalComponent } from './equipment modal/equipment-modal';
import { CalendarComponent } from './Calendar/calendar.component';
// Ensure correct path

export const EquipmentRoutes: Routes = [
  // {
  //   path: '',
  //   component: EquipmentDetailsComponent, // Correct component reference
  // },
  {
      path: '',
      children: [
        {
          path: 'calendar',
          component:CalendarComponent,
        },
        {
          path: 'equipment-list',
          component: EquipmentComponent,
        },
        {
          path:'equipment-details/:id',
          component:EquipmentDetailsComponent
        }
        
        
      ],
    },
];
