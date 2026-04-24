import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component')
            .then(m => m.DashboardComponent)
      },

      {
        path: 'patients',
        loadChildren: () =>
          import('./features/patients/patients.routes')
            .then(m => m.PATIENT_ROUTES)
      },

      {
        path: 'appointments',
        loadComponent: () =>
          import('./features/appointments/appointments.component')
            .then(m => m.AppointmentsComponent)
      },

      {
        path: 'doctors',
        loadComponent: () =>
          import('./features/doctors/doctors.component')
            .then(m => m.DoctorsComponent)
      },

      {
        path: 'records',
        loadComponent: () =>
          import('./features/records/records.component')
            .then(m => m.RecordsComponent)
      },

      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      }
    ]
  }
];