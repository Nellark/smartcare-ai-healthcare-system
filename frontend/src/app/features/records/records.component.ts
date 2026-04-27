import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MedicalRecordService } from '../../core/services/medical-record.service';
import { PatientService } from '../../core/services/patient.service';
import { DoctorService } from '../../core/services/doctor.service';
import { MedicalRecord, UpsertMedicalRecordRequest } from '../../core/models/medical-record.model';
import { Patient } from '../../core/models/patient.model';
import { Doctor } from '../../core/models/doctor.model';

@Component({
  selector: 'app-records',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './records.component.html',
  styleUrls: ['./records.component.css']
})
export class RecordsComponent implements OnInit {
  records: MedicalRecord[] = [];
  patients: Patient[] = [];
  doctors: Doctor[] = [];
  isLoading = false;
  errorMessage = '';

  filterPatientId = '';
  editingRecordId: string | null = null;

  recordForm: UpsertMedicalRecordRequest = {
    patientId: '',
    diagnosis: '',
    treatment: '',
    notes: '',
    recordDate: '',
    doctorId: ''
  };

  constructor(
    private readonly medicalRecordService: MedicalRecordService,
    private readonly patientService: PatientService,
    private readonly doctorService: DoctorService
  ) {}

  ngOnInit(): void {
    this.loadReferenceData();
    this.loadRecords();
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

  loadRecords(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.medicalRecordService.getAll(this.filterPatientId || null).subscribe({
      next: (response) => {
        if (response.success) {
          this.records = response.data ?? [];
        } else {
          this.errorMessage = response.message || 'Failed to load medical records';
          this.records = [];
        }
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'An error occurred while loading medical records';
        this.records = [];
        this.isLoading = false;
      }
    });
  }

  onFilterChange(): void {
    this.loadRecords();
  }

  resetForm(): void {
    this.editingRecordId = null;
    this.recordForm = {
      patientId: '',
      diagnosis: '',
      treatment: '',
      notes: '',
      recordDate: '',
      doctorId: ''
    };
  }

  editRecord(record: MedicalRecord): void {
    this.editingRecordId = record.id;
    this.recordForm = {
      patientId: record.patientId,
      diagnosis: record.diagnosis,
      treatment: record.treatment,
      notes: record.notes,
      recordDate: this.toInputDateTime(record.recordDate),
      doctorId: record.doctorId
    };
  }

  saveRecord(): void {
    const request$ = this.editingRecordId
      ? this.medicalRecordService.update(this.editingRecordId, this.recordForm)
      : this.medicalRecordService.create(this.recordForm);

    request$.subscribe({
      next: (response) => {
        if (response.success) {
          this.loadRecords();
          this.resetForm();
        } else {
          this.errorMessage = response.message || 'Unable to save medical record';
        }
      },
      error: () => {
        this.errorMessage = 'An error occurred while saving the medical record';
      }
    });
  }

  deleteRecord(id: string): void {
    if (!confirm('Delete this medical record?')) {
      return;
    }

    this.medicalRecordService.delete(id).subscribe({
      next: (response) => {
        if (response.success) {
          this.loadRecords();
        } else {
          this.errorMessage = response.message || 'Unable to delete medical record';
        }
      },
      error: () => {
        this.errorMessage = 'An error occurred while deleting the medical record';
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
    return new Date(value).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    });
  }

  private toInputDateTime(value: string): string {
    return new Date(value).toISOString().slice(0, 16);
  }
}
