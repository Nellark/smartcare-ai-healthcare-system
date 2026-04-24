import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ModalComponent } from '../../../shared/components/modal/modal.component';
import { PatientService } from '../../../core/services/patient.service';
import { CreatePatientRequest, Patient } from '../../../core/models/patient.model';

@Component({
  selector: 'app-patient-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, ModalComponent],
  templateUrl: './patient-modal.component.html',
  styleUrls: ['./patient-modal.component.css']
})
export class PatientModalComponent {
  @Input() isVisible = false;
  @Input() patient: Patient | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() patientSaved = new EventEmitter<Patient>();

  patientForm: FormGroup;
  isEditMode = false;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private patientService: PatientService
  ) {
    this.patientForm = this.createPatientForm();
  }

  private createPatientForm(): FormGroup {
    return this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email]],
      dateOfBirth: ['', [Validators.required]],
      phoneNumber: ['', [Validators.required, Validators.maxLength(20)]],
      address: ['', [Validators.required, Validators.maxLength(200)]]
    });
  }

  ngOnChanges(): void {
    if (this.isVisible) {
      this.resetForm();
      if (this.patient) {
        this.isEditMode = true;
        this.populateForm();
      } else {
        this.isEditMode = false;
      }
    }
  }

  private resetForm(): void {
    this.patientForm.reset();
    this.errorMessage = '';
    this.isLoading = false;
  }

  private populateForm(): void {
    if (this.patient) {
      this.patientForm.patchValue({
        firstName: this.patient.firstName,
        lastName: this.patient.lastName,
        email: this.patient.email,
        dateOfBirth: this.formatDateForInput(this.patient.dateOfBirth),
        phoneNumber: this.patient.phoneNumber,
        address: this.patient.address
      });
    }
  }

  private formatDateForInput(dateString: string): string {
    const date = new Date(dateString);
    return date.toISOString().split('T')[0];
  }

  onClose(): void {
    this.close.emit();
  }

  onSave(): void {
    if (this.patientForm.invalid) {
      this.markFormGroupTouched(this.patientForm);
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const patientData: CreatePatientRequest = this.patientForm.value;

    if (this.isEditMode && this.patient) {
      this.patientService.update(this.patient.id, patientData).subscribe({
        next: (response) => {
          if (response.success) {
            this.patientSaved.emit(response.data);
            this.onClose();
          } else {
            this.errorMessage = response.message || 'Failed to update patient';
          }
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'An error occurred while updating the patient';
          this.isLoading = false;
        }
      });
    } else {
      this.patientService.create(patientData).subscribe({
        next: (response) => {
          if (response.success) {
            this.patientSaved.emit(response.data);
            this.onClose();
          } else {
            this.errorMessage = response.message || 'Failed to create patient';
          }
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'An error occurred while creating the patient';
          this.isLoading = false;
        }
      });
    }
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  get firstName() { return this.patientForm.get('firstName'); }
  get lastName() { return this.patientForm.get('lastName'); }
  get email() { return this.patientForm.get('email'); }
  get dateOfBirth() { return this.patientForm.get('dateOfBirth'); }
  get phoneNumber() { return this.patientForm.get('phoneNumber'); }
  get address() { return this.patientForm.get('address'); }
}
