import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DoctorService } from '../../core/services/doctor.service';
import { Doctor, UpsertDoctorRequest } from '../../core/models/doctor.model';

@Component({
  selector: 'app-doctors',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './doctors.component.html',
  styleUrls: ['./doctors.component.css']
})
export class DoctorsComponent implements OnInit {
  doctors: Doctor[] = [];
  isLoading = false;
  errorMessage = '';
  searchTerm = '';
  editingDoctorId: string | null = null;

  doctorForm: UpsertDoctorRequest = {
    firstName: '',
    lastName: '',
    email: '',
    specialty: '',
    phoneNumber: '',
    licenseNumber: ''
  };

  constructor(private readonly doctorService: DoctorService) {}

  ngOnInit(): void {
    this.loadDoctors();
  }

  loadDoctors(): void {
    this.isLoading = true;
    this.errorMessage = '';

    const request$ = this.searchTerm.trim()
      ? this.doctorService.searchBySpecialty(this.searchTerm.trim())
      : this.doctorService.getAll();

    request$.subscribe({
      next: (response) => {
        if (response.success) {
          this.doctors = response.data ?? [];
        } else {
          this.errorMessage = response.message || 'Failed to load doctors';
          this.doctors = [];
        }
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'An error occurred while loading doctors';
        this.doctors = [];
        this.isLoading = false;
      }
    });
  }

  onSearchChange(): void {
    this.loadDoctors();
  }

  resetForm(): void {
    this.editingDoctorId = null;
    this.doctorForm = {
      firstName: '',
      lastName: '',
      email: '',
      specialty: '',
      phoneNumber: '',
      licenseNumber: ''
    };
  }

  editDoctor(doctor: Doctor): void {
    this.editingDoctorId = doctor.id;
    this.doctorForm = {
      firstName: doctor.firstName,
      lastName: doctor.lastName,
      email: doctor.email,
      specialty: doctor.specialty,
      phoneNumber: doctor.phoneNumber,
      licenseNumber: doctor.licenseNumber
    };
  }

  saveDoctor(): void {
    const request$ = this.editingDoctorId
      ? this.doctorService.update(this.editingDoctorId, this.doctorForm)
      : this.doctorService.create(this.doctorForm);

    request$.subscribe({
      next: (response) => {
        if (response.success) {
          this.loadDoctors();
          this.resetForm();
        } else {
          this.errorMessage = response.message || 'Unable to save doctor';
        }
      },
      error: () => {
        this.errorMessage = 'An error occurred while saving the doctor';
      }
    });
  }

  deleteDoctor(id: string): void {
    if (!confirm('Delete this doctor?')) {
      return;
    }

    this.doctorService.delete(id).subscribe({
      next: (response) => {
        if (response.success) {
          this.loadDoctors();
        } else {
          this.errorMessage = response.message || 'Unable to delete doctor';
        }
      },
      error: () => {
        this.errorMessage = 'An error occurred while deleting the doctor';
      }
    });
  }
}
