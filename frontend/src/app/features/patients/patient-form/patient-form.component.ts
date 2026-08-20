import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { PatientService } from '../../../core/services/patient.service';
import { ToastService } from '../../../core/services/toast.service';
import { Patient, CreatePatientRequest } from '../../../core/models/patient.model';

@Component({
  selector: 'app-patient-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './patient-form.component.html',
  styleUrls: ['./patient-form.component.css']
})
export class PatientFormComponent implements OnInit {
  private patientService = inject(PatientService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private toast = inject(ToastService);

  patient: CreatePatientRequest = {
    firstName: '',
    lastName: '',
    email: '',
    dateOfBirth: '',
    phoneNumber: '',
    address: ''
  };

  isEditMode = false;
  patientId: string | null = null;
  isLoading = false;
  isSaving = false;
  loadError = '';

  ngOnInit(): void {
    this.patientId = this.route.snapshot.paramMap.get('id');
    if (this.patientId) {
      this.isEditMode = true;
      this.loadPatient(this.patientId);
    }
  }

  loadPatient(id: string): void {
    this.isLoading = true;
    this.loadError = '';
    this.patientService.getById(id).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          const d = response.data;
          this.patient = {
            firstName: d.firstName,
            lastName: d.lastName,
            email: d.email,
            dateOfBirth: d.dateOfBirth ? d.dateOfBirth.substring(0, 10) : '',
            phoneNumber: d.phoneNumber,
            address: d.address
          };
        } else {
          this.loadError = response.message || 'Failed to load patient data.';
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.loadError = err.status === 401 || err.status === 403
          ? 'Session expired. Please log out and sign in again.'
          : 'Could not load patient. Ensure the backend is running.';
        this.isLoading = false;
      }
    });
  }

  save(): void {
    if (!this.patient.firstName.trim() || !this.patient.lastName.trim() || !this.patient.email.trim()) {
      this.toast.warning('Missing fields', 'First name, last name and email are required.');
      return;
    }

    this.isSaving = true;

    if (this.isEditMode && this.patientId) {
      this.patientService.update(this.patientId, this.patient).subscribe({
        next: (response) => {
          this.isSaving = false;
          if (response.success) {
            this.toast.success('Patient updated', `${this.patient.firstName} ${this.patient.lastName} has been updated.`);
            this.router.navigate(['/app/patients']);
          } else {
            this.toast.error('Update failed', response.message);
          }
        },
        error: () => {
          this.isSaving = false;
          this.toast.error('Update failed', 'Could not reach the API. Please ensure the backend is running.');
        }
      });
    } else {
      this.patientService.create(this.patient).subscribe({
        next: (response) => {
          this.isSaving = false;
          if (response.success) {
            this.toast.success('Patient created', `${this.patient.firstName} ${this.patient.lastName} has been added.`);
            this.router.navigate(['/app/patients']);
          } else {
            this.toast.error('Creation failed', response.message);
          }
        },
        error: () => {
          this.isSaving = false;
          this.toast.error('Creation failed', 'Could not reach the API. Please ensure the backend is running.');
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/app/patients']);
  }
}
