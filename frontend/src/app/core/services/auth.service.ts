import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { Observable, tap } from 'rxjs';

export interface LoginResponse {
  AccessToken?: string;
  accessToken?: string;
  ExpiresAtUtc?: string;
  expiresAtUtc?: string;
  Email?: string;
  email?: string;
  Role?: string;
  role?: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

type KnownRole = 'Patient' | 'Doctor' | 'Nurse' | 'Admin';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly baseUrl = `${environment.apiBaseUrl}/auth`;
  private readonly adminEmail = 'admin@smartcare.local';
  
  // Use a signal to hold the current user email/token state
  public currentUser = signal<string | null>(null);
  public currentUserRole = signal<string | null>(null);

  constructor(private http: HttpClient, private router: Router) {
    this.checkToken();
  }

  private isAdminEmail(email: string | null | undefined): boolean {
    return (email ?? '').trim().toLowerCase() === this.adminEmail;
  }

  private normalizeRole(role: string | null | undefined): string | null {
    if (!role) {
      return null;
    }

    const lowerRole = role.trim().toLowerCase();
    if (!lowerRole) {
      return null;
    }

    const roleMap: Record<string, KnownRole> = {
      patient: 'Patient',
      doctor: 'Doctor',
      provider: 'Doctor',
      admin: 'Admin',
      nurse: 'Nurse'
    };

    return roleMap[lowerRole] ?? role.trim();
  }

  getRoleLabel(role: string | null | undefined = this.currentUserRole()): string {
    if (this.isAdminEmail(this.currentUser())) {
      return 'Admin';
    }

    switch (this.normalizeRole(role)) {
      case 'Doctor':
        return 'Provider';
      case 'Admin':
        return 'Admin';
      case 'Nurse':
        return 'Nurse';
      case 'Patient':
        return 'Patient';
      default:
        return 'User';
    }
  }

  isPatientRole(): boolean {
    return this.normalizeRole(this.currentUserRole()) === 'Patient';
  }

  isProviderRole(): boolean {
    if (this.isAdminEmail(this.currentUser())) {
      return true;
    }

    const role = this.normalizeRole(this.currentUserRole());
    return role === 'Doctor' || role === 'Nurse' || role === 'Admin';
  }

  isAdminRole(): boolean {
    return this.isAdminEmail(this.currentUser()) || this.normalizeRole(this.currentUserRole()) === 'Admin';
  }

  private checkToken() {
    const token = localStorage.getItem('token');
    if (token) {
      // Decode token to get email if needed, or just set it as present
      try {
        let base64 = token.split('.')[1];
        base64 = base64.replace(/-/g, '+').replace(/_/g, '/');
        const pad = base64.length % 4;
        if (pad) {
          base64 += new Array(5 - pad).join('=');
        }
        const payload = JSON.parse(atob(base64));
        console.log('Decoded JWT payload:', payload);
        this.currentUser.set(payload.email || payload.sub || 'User');
        
        // Extract role from standard JWT claim, lowercase 'role', or uppercase 'Role'
        const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] 
                  || payload.role 
                  || payload.Role 
                  || null;
        
        console.log('Extracted role:', role);
        this.currentUserRole.set(this.isAdminEmail(payload.email || payload.sub) ? 'Admin' : this.normalizeRole(role));
      } catch (e) {
        console.error('Error decoding token:', e);
        this.currentUser.set('User');
        this.currentUserRole.set(null);
      }
    } else {
      this.currentUser.set(null);
      this.currentUserRole.set(null);
    }
  }

  login(credentials: any): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.baseUrl}/login`, credentials)
      .pipe(
        tap(response => {
          console.log('Login response:', response);
          const token = response.data?.accessToken || (response.data as any)?.AccessToken || (response.data as any)?.token;
          if (response.success && token) {
            this.setToken(token);
          }
        })
      );
  }

  register(userData: any): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.baseUrl}/register`, userData)
      .pipe(
        tap(response => {
          const token = (response.data as any)?.accessToken || (response.data as any)?.AccessToken || (response.data as any)?.token;
          if (response.success && token) {
            this.setToken(token);
          }
        })
      );
  }

  createUserByAdmin(userData: any): Observable<ApiResponse<unknown>> {
    return this.http.post<ApiResponse<unknown>>(`${this.baseUrl}/admin/create-user`, userData, {
      headers: {
        Authorization: `Bearer ${this.getToken()}`
      }
    });
  }

  logout(): void {
    localStorage.removeItem('token');
    this.currentUser.set(null);
    this.currentUserRole.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    const token = localStorage.getItem('token');
    console.log('Getting token:', token ? 'Found' : 'Not found');
    return token;
  }

  isAuthenticated(): boolean {
    const authed = !!this.getToken();
    console.log('Is authenticated:', authed);
    return authed;
  }

  private setToken(token: string): void {
    console.log('Storing token to localStorage');
    localStorage.setItem('token', token);
    this.checkToken();
  }
}
