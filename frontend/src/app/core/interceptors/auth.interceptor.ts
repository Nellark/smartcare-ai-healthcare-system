import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  console.log(`[Interceptor] ${req.method} ${req.url} - Token: ${token ? 'Present' : 'Missing'}`);

  if (token) {
    const clonedReq = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${token}`)
    });
    console.log(`[Interceptor] Added Authorization header to request`);
    return next(clonedReq);
  }

  console.log(`[Interceptor] No token available, sending request without authorization`);
  return next(req);
};
