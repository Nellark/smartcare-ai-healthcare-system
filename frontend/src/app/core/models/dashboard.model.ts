import { Patient } from './patient.model';
import { Appointment } from './appointment.model';

export interface DashboardStats {
  totalPatients: number;
  totalDoctors: number;
  totalAppointments: number;
  recentPatients: Patient[];
  recentAppointments: Appointment[];
}
