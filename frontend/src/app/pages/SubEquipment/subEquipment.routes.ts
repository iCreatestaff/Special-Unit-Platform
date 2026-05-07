import { Routes } from '@angular/router';

import { SubEquipmentAddComponent } from './sub equipment list/sub-equipment-add.component';
import { EditSubEquipmentDialogComponent } from './sub equipment modal/edit-sub-equipment.component';
import { DeleteConfirmationDialogComponent } from './sub equipment delete/delete-confirmation.component';
// Ensure correct path

export const SubEquipmentRoutes: Routes = [

  {
      path: '',
      children: [
        {
          path: 'add-sub-equipment',
          component: SubEquipmentAddComponent,
        },
        {
          path: 'edit-sub-equipment/:id',
          component: EditSubEquipmentDialogComponent,
        },
        {
          path: 'delete-sub-equipment',
          component: DeleteConfirmationDialogComponent,
        },
       
        
      ],
    },
];
