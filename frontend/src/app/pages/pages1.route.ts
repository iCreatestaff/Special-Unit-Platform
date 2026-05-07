import { Routes } from '@angular/router';
import { StarterComponent } from './starter/starter.component';
import { CalendarComponent } from './Equipment/Calendar/calendar.component';
import { ProfileComponent } from './profile/profile.component';

export const Pages1Routes: Routes = [
  {
    path: '',
    component: ProfileComponent,
    data: {
      title: 'Profile',
      urls: [
        { title: 'Profile', url: '/profile' },
        { title: 'Profile' },
      ],
    },
  },
  
  
];
