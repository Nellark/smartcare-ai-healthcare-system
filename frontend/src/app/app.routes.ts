import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { authGuard } from './core/guards/auth.guard';
import { providerGuard, patientGuard, dashboardRedirectGuard } from './core/guards/role.guard';

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
        canActivate: [providerGuard],
        loadComponent: () =>
          import('./features/dashboard/dashboard.component')
            .then(m => m.DashboardComponent)
      },

      {
        path: 'patients',
        canActivate: [providerGuard],
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
        path: 'messages',
        loadComponent: () =>
          import('./features/messages/messages.component')
            .then(m => m.MessagesComponent)
      },

      {
        path: 'billing',
        loadComponent: () =>
          import('./features/billing/billing.component')
            .then(m => m.BillingComponent)
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
        path: 'settings',
        loadComponent: () =>
          import('./features/settings/settings.component')
            .then(m => m.SettingsComponent)
      },

      {
        path: 'new-consultation',
        loadComponent: () =>
          import('./features/new-consultation/new-consultation.component')
            .then(m => m.NewConsultationComponent)
      },

      {
        path: 'patient-dashboard',
        canActivate: [patientGuard],
        loadComponent: () =>
          import('./features/patient-dashboard/patient-dashboard.component')
            .then(m => m.PatientDashboardComponent)
      },

      {
        path: '',
        canActivate: [dashboardRedirectGuard],
        children: []
      }
    ]
  }
];