import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const providerGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  if (authService.currentUserRole() !== 'Patient') {
    return true;
  }
  return router.parseUrl('/app/patient-dashboard');
};

export const patientGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  if (authService.currentUserRole() === 'Patient') {
    return true;
  }
  return router.parseUrl('/app/dashboard');
};

export const dashboardRedirectGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  if (authService.currentUserRole() === 'Patient') {
    return router.parseUrl('/app/patient-dashboard');
  }
  return router.parseUrl('/app/dashboard');
};
