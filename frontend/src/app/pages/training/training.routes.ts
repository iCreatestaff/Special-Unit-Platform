import { Routes } from '@angular/router';
import { TrainingListComponent } from './training_list/training-list.component';


export const TrainingRoutes: Routes = [

  {
      path: '',
      children: [
        {
          path: 'training-list',
          component: TrainingListComponent,
        }
        
      ],
    },
];
