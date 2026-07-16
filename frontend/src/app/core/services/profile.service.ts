import { Injectable, signal, inject } from '@angular/core';
import { AuthService } from './auth.service';

export interface UserProfile {
  name: string;
  role: string;
  location: string;
  avatar: string;
  fullName: string;
  email: string;
  contact: string;
  office: string;
  hours: string;
}

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private authService = inject(AuthService);

  // Load from localStorage or use defaults
  private loadProfile(): UserProfile {
    const saved = localStorage.getItem('userProfile');
    if (saved) {
      try {
        return JSON.parse(saved);
      } catch (e) {
        // Fallback to default
      }
    }
    
    const email = this.authService.currentUser() || 'user@example.com';
    const role = this.authService.currentUserRole() || 'Patient';
    const namePart = email.split('@')[0];
    const name = namePart.split('.').map(p => p.charAt(0).toUpperCase() + p.slice(1)).join(' ');

    return {
      name: role === 'Patient' ? name : `Dr. ${name}`,
      role: role,
      location: "SmartCare Clinic",
      avatar: '',
      fullName: role === 'Patient' ? name : `Dr. ${name}, M.D.`,
      email: email,
      contact: 'Not provided',
      office: 'Not provided',
      hours: 'Not provided'
    };
  }

  private _profile = signal<UserProfile>(this.loadProfile());
  readonly currentProfile = this._profile.asReadonly();

  updateProfile(newProfile: Partial<UserProfile>) {
    const updated = { ...this._profile(), ...newProfile };
    this._profile.set(updated);
    localStorage.setItem('userProfile', JSON.stringify(updated));
  }
}
