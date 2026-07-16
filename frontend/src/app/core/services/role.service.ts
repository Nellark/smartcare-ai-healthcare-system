import { Injectable, signal, computed } from '@angular/core';

export type UserRole = 'patient' | 'provider';

@Injectable({ providedIn: 'root' })
export class RoleService {
  private _role = signal<UserRole>(this.loadRole());

  readonly currentRole = this._role.asReadonly();
  readonly isPatient   = computed(() => this._role() === 'patient');
  readonly isProvider  = computed(() => this._role() === 'provider');

  setRole(role: UserRole): void {
    this._role.set(role);
    localStorage.setItem('smartcare_role', role);
  }

  private loadRole(): UserRole {
    const saved = localStorage.getItem('smartcare_role');
    return (saved === 'patient' || saved === 'provider') ? saved : 'provider';
  }
}
