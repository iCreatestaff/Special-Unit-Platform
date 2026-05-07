import { Routes } from '@angular/router';

import { AccountComponent } from './account list/account.component';
import {  MessagePageComponent } from './Account Message Modal/message-modal.component';
// Ensure correct path

export const AccountRoutes: Routes = [
 
  {
      path: '',
      children: [
        {
          path: 'account-list',
          component:AccountComponent,
        },
        
        {
          path: 'app-message-page',
          component:MessagePageComponent,
        }
        
      ],
    },
];
