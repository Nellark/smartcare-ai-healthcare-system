import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Patient, ApiResponse } from '../models/patient.model';

export interface DashboardStats {
  totalPatients: number;
  totalAppointments: number;
  totalDoctors: number;
  recentPatients: Patient[];
}

export interface DashboardStatsResponse extends ApiResponse<DashboardStats> {}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private baseUrl = 'http://localhost:5255/api';

  constructor(private http: HttpClient) {}

  getDashboardStats(): Observable<DashboardStatsResponse> {
    // For now, we'll calculate stats from patient data
    // In a real app, you might have a dedicated dashboard endpoint
    return this.http.get<DashboardStatsResponse>(`${this.baseUrl}/patients`);
  }

  calculateStats(patients: Patient[]): DashboardStats {
    return {
      totalPatients: patients.length,
      totalAppointments: this.calculateTotalAppointments(patients),
      totalDoctors: this.calculateTotalDoctors(patients),
      recentPatients: patients.slice(-5).reverse() // Last 5 patients
    };
  }

  private calculateTotalAppointments(patients: Patient[]): number {
    // For now, estimate appointments based on medical records
    return patients.reduce((total, patient) => {
      return total + (patient.medicalRecords?.length || 0);
    }, 0);
  }

  private calculateTotalDoctors(patients: Patient[]): number {
    // Count unique doctor IDs from medical records
    const doctorIds = new Set<string>();
    patients.forEach(patient => {
      patient.medicalRecords?.forEach(record => {
        doctorIds.add(record.doctorId);
      });
    });
    return doctorIds.size;
  }
}
