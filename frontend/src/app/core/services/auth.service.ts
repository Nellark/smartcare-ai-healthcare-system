import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { Observable, tap } from 'rxjs';

export interface LoginResponse {
  accessToken: string;
  message: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly baseUrl = `${environment.apiBaseUrl}/auth`;
  
  // Use a signal to hold the current user email/token state
  public currentUser = signal<string | null>(null);

  constructor(private http: HttpClient, private router: Router) {
    this.checkToken();
  }

  private checkToken() {
    const token = localStorage.getItem('token');
    if (token) {
      // Decode token to get email if needed, or just set it as present
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        this.currentUser.set(payload.email || payload.sub || 'User');
      } catch {
        this.currentUser.set('User');
      }
    }
  }

  login(credentials: any): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.baseUrl}/login`, credentials)
      .pipe(
        tap(response => {
          if (response.success && response.data?.accessToken) {
            this.setToken(response.data.accessToken);
          }
        })
      );
  }

  register(userData: any): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.baseUrl}/register`, userData)
      .pipe(
        tap(response => {
          if (response.success && response.data?.accessToken) {
            this.setToken(response.data.accessToken);
          }
        })
      );
  }

  logout(): void {
    localStorage.removeItem('token');
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  private setToken(token: string): void {
    localStorage.setItem('token', token);
    this.checkToken();
  }
}
