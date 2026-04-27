import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AppointmentService } from '../../core/services/appointment.service';
import { PatientService } from '../../core/services/patient.service';
import { DoctorService } from '../../core/services/doctor.service';
import { Appointment, AppointmentStatus, UpsertAppointmentRequest } from '../../core/models/appointment.model';
import { Patient } from '../../core/models/patient.model';
import { Doctor } from '../../core/models/doctor.model';

@Component({
  selector: 'app-appointments',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './appointments.component.html',
  styleUrls: ['./appointments.component.css']
})
export class AppointmentsComponent implements OnInit {
  appointments: Appointment[] = [];
  patients: Patient[] = [];
  doctors: Doctor[] = [];
  isLoading = false;
  errorMessage = '';

  filterPatientId = '';
  filterDoctorId = '';
  editingAppointmentId: string | null = null;

  appointmentForm: UpsertAppointmentRequest = {
    patientId: '',
    doctorId: '',
    scheduledAt: '',
    durationMinutes: 30,
    reason: '',
    notes: '',
    status: 'Scheduled'
  };

  readonly statuses: AppointmentStatus[] = ['Scheduled', 'Confirmed', 'Completed', 'Cancelled', 'NoShow'];

  constructor(
    private readonly appointmentService: AppointmentService,
    private readonly patientService: PatientService,
    private readonly doctorService: DoctorService
  ) {}

  ngOnInit(): void {
    this.loadReferenceData();
    this.loadAppointments();
  }

  loadReferenceData(): void {
    this.patientService.getAll().subscribe({
      next: (response) => {
        this.patients = response.success ? response.data ?? [] : [];
      },
      error: () => {
        this.patients = [];
      }
    });

    this.doctorService.getAll().subscribe({
      next: (response) => {
        this.doctors = response.success ? response.data ?? [] : [];
      },
      error: () => {
        this.doctors = [];
      }
    });
  }

  loadAppointments(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.appointmentService.getAll(this.filterPatientId || null, this.filterDoctorId || null).subscribe({
      next: (response) => {
        if (response.success) {
          this.appointments = response.data ?? [];
        } else {
          this.errorMessage = response.message || 'Failed to load appointments';
          this.appointments = [];
        }
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'An error occurred while loading appointments';
        this.appointments = [];
        this.isLoading = false;
      }
    });
  }

  onFilterChange(): void {
    this.loadAppointments();
  }

  resetForm(): void {
    this.editingAppointmentId = null;
    this.appointmentForm = {
      patientId: '',
      doctorId: '',
      scheduledAt: '',
      durationMinutes: 30,
      reason: '',
      notes: '',
      status: 'Scheduled'
    };
  }

  editAppointment(appointment: Appointment): void {
    this.editingAppointmentId = appointment.id;
    this.appointmentForm = {
      patientId: appointment.patientId,
      doctorId: appointment.doctorId,
      scheduledAt: this.toInputDateTime(appointment.scheduledAt),
      durationMinutes: appointment.durationMinutes,
      reason: appointment.reason,
      notes: appointment.notes,
      status: appointment.status
    };
  }

  saveAppointment(): void {
    const request$ = this.editingAppointmentId
      ? this.appointmentService.update(this.editingAppointmentId, this.appointmentForm)
      : this.appointmentService.create(this.appointmentForm);

    request$.subscribe({
      next: (response) => {
        if (response.success) {
          this.loadAppointments();
          this.resetForm();
        } else {
          this.errorMessage = response.message || 'Unable to save appointment';
        }
      },
      error: () => {
        this.errorMessage = 'An error occurred while saving the appointment';
      }
    });
  }

  deleteAppointment(id: string): void {
    if (!confirm('Delete this appointment?')) {
      return;
    }

    this.appointmentService.delete(id).subscribe({
      next: (response) => {
        if (response.success) {
          this.loadAppointments();
        } else {
          this.errorMessage = response.message || 'Unable to delete appointment';
        }
      },
      error: () => {
        this.errorMessage = 'An error occurred while deleting the appointment';
      }
    });
  }

  getPatientName(patientId: string): string {
    const patient = this.patients.find(item => item.id === patientId);
    return patient ? `${patient.firstName} ${patient.lastName}` : patientId;
  }

  getDoctorName(doctorId: string): string {
    const doctor = this.doctors.find(item => item.id === doctorId);
    return doctor ? `${doctor.firstName} ${doctor.lastName}` : doctorId;
  }

  formatDate(value: string): string {
    return new Date(value).toLocaleString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: 'numeric',
      minute: '2-digit'
    });
  }

  private toInputDateTime(value: string): string {
    return new Date(value).toISOString().slice(0, 16);
  }
}
