export type AppointmentStatus = 'Scheduled' | 'Confirmed' | 'Completed' | 'Cancelled' | 'NoShow';

export interface Appointment {
  id: string;
  patientId: string;
  doctorId: string;
  scheduledAt: string;
  durationMinutes: number;
  reason: string;
  notes: string;
  status: AppointmentStatus;
  createdAt: string;
  updatedAt?: string | null;
}

export interface UpsertAppointmentRequest {
  patientId: string;
  doctorId: string;
  scheduledAt: string;
  durationMinutes: number;
  reason: string;
  notes: string;
  status: AppointmentStatus;
}
