import { Injectable, signal } from '@angular/core';

export interface BookedAppointment {
  id: string;
  providerName: string;
  providerSpecialty: string;
  providerAvatar: string;
  date: Date;
  timeLabel: string;
  timeAmpm: string;
  reason: string;
  notes: string;
  status: 'Upcoming' | 'Completed' | 'Cancelled';
  type: 'in-person' | 'telehealth';
}

@Injectable({ providedIn: 'root' })
export class ConsultationStoreService {
  private _appointments = signal<BookedAppointment[]>([]);

  readonly appointments = this._appointments.asReadonly();

  addAppointment(appt: Omit<BookedAppointment, 'id'>): void {
    const id = 'apt-' + Date.now();
    this._appointments.update(list => [{ id, ...appt }, ...list]);
  }

  cancelAppointment(id: string): void {
    this._appointments.update(list =>
      list.map(a => a.id === id ? { ...a, status: 'Cancelled' as const } : a)
    );
  }
}
