import { Injectable, signal, inject, computed } from '@angular/core';
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
  private updateTrigger = signal<number>(0);

  // Dynamic profile computed based on current user email and role
  readonly currentProfile = computed<UserProfile>(() => {
    this.updateTrigger(); // Reactive dependency for manual updates
    
    const email = this.authService.currentUser();
    if (!email) {
      return {
        name: 'User',
        role: 'Patient',
        location: 'SmartCare Clinic, Johannesburg',
        avatar: '',
        fullName: 'User',
        email: '',
        contact: '',
        office: '',
        hours: ''
      };
    }

    const role = this.authService.currentUserRole() || 'Patient';
    const saved = localStorage.getItem(`userProfile_${email}`);
    if (saved) {
      try {
        return JSON.parse(saved);
      } catch (e) {
        // Fallback to default
      }
    }

    const namePart = email.split('@')[0];
    const name = namePart.split('.').map(p => p.charAt(0).toUpperCase() + p.slice(1)).join(' ');

    return {
      name: role === 'Patient' ? name : `Dr. ${name}`,
      role: role,
      location: "SmartCare Clinic, Johannesburg",
      avatar: '',
      fullName: role === 'Patient' ? name : `Dr. ${name}, M.D.`,
      email: email,
      contact: '+27 (82) 123-4567',
      office: 'Block B, Nelson Mandela Square, Sandton, 2196',
      hours: 'Mon-Fri 08:00 - 17:00'
    };
  });

  updateProfile(newProfile: Partial<UserProfile>) {
    const email = this.authService.currentUser();
    if (!email) return;

    const current = this.currentProfile();
    const updated = { ...current, ...newProfile };
    localStorage.setItem(`userProfile_${email}`, JSON.stringify(updated));
    this.updateTrigger.update(n => n + 1);
  }
}
