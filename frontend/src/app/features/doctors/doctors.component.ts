import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DoctorService } from '../../core/services/doctor.service';
import { Doctor, UpsertDoctorRequest } from '../../core/models/doctor.model';
import { ToastService } from '../../core/services/toast.service';
import { inject } from '@angular/core';
import { ModalComponent } from '../../shared/components/modal/modal.component';

@Component({
  selector: 'app-doctors',
  standalone: true,
  imports: [CommonModule, FormsModule, ModalComponent],
  templateUrl: './doctors.component.html',
  styleUrls: ['./doctors.component.css']
})
export class DoctorsComponent implements OnInit {
  doctors: Doctor[] = [];
  isLoading = false;
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

  private toast = inject(ToastService);
  
  constructor(private readonly doctorService: DoctorService) {}

  ngOnInit(): void {
    this.loadDoctors();
  }

  loadDoctors(): void {
    this.isLoading = true;

    const request$ = this.searchTerm.trim()
      ? this.doctorService.searchBySpecialty(this.searchTerm.trim())
      : this.doctorService.getAll();

    request$.subscribe({
      next: (response) => {
        if (response.success) {
          this.doctors = response.data ?? [];
        } else {
          this.toast.error('Error', response.message || 'Failed to load doctors');
          this.doctors = [];
        }
        this.isLoading = false;
      },
      error: () => {
        this.toast.error('Error', 'An error occurred while loading doctors');
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
          this.toast.success('Success', 'Doctor saved successfully');
          this.loadDoctors();
          this.resetForm();
        } else {
          this.toast.error('Error', response.message || 'Unable to save doctor');
        }
      },
      error: () => {
        this.toast.error('Error', 'An error occurred while saving the doctor');
      }
    });
  }

  isDeleteModalVisible = false;
  doctorToDelete: Doctor | null = null;

  confirmDeleteDoctor(doctor: Doctor): void {
    this.doctorToDelete = doctor;
    this.isDeleteModalVisible = true;
  }

  closeDeleteModal(): void {
    this.isDeleteModalVisible = false;
    this.doctorToDelete = null;
  }

  deleteDoctor(): void {
    if (!this.doctorToDelete) return;
    
    this.doctorService.delete(this.doctorToDelete.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.toast.success('Success', 'Doctor deleted successfully');
          this.loadDoctors();
          this.closeDeleteModal();
        } else {
          this.toast.error('Error', response.message || 'Unable to delete doctor');
        }
      },
      error: (error) => {
        let errorMessage = 'An error occurred while deleting the doctor';
        if (error.error && error.error.message) {
          errorMessage = error.error.message;
        } else if (error.message) {
          errorMessage = error.message;
        }
        this.toast.error('Error', errorMessage);
      }
    });
  }
}
