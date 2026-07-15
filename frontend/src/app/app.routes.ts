import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/landing/landing.component').then(m => m.LandingComponent)
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'onboarding',
    canActivate: [authGuard],
    loadComponent: () => import('./features/onboarding/onboarding.component').then(m => m.OnboardingComponent)
  },
  {
    path: 'app',
    component: MainLayoutComponent,
    canActivate: [authGuard],
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
        path: 'analytics',
        loadComponent: () =>
          import('./features/analytics/analytics.component')
            .then(m => m.AnalyticsComponent)
      },

      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      }
    ]
  }
];