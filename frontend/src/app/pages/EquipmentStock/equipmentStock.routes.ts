import { Routes } from '@angular/router';
import { EquipmentStockListComponent } from './equipmentStock list/equipment-stock.component';


export const EquipmentStockRoutes: Routes = [
  
  {
      path: '',
      children: [
        {
          path: 'equipmentStock-list',
          component:EquipmentStockListComponent,
        },
        
        
      ],
    },
];
