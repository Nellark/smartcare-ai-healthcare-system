import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PatientService } from '../../../core/services/patient.service';
import { Patient, CreatePatientRequest } from '../../../core/models/patient.model';

@Component({
  selector: 'app-patient-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './patient-form.component.html',
  styleUrls: ['./patient-form.component.css']
})
export class PatientFormComponent implements OnInit {

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

  constructor(
    private patientService: PatientService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.patientId = this.route.snapshot.paramMap.get('id');

    if (this.patientId) {
      this.isEditMode = true;
      this.loadPatient(this.patientId);
    }
  }

  loadPatient(id: string) {
    this.patientService.getById(id).subscribe(response => {
      if (response.success) {
        const patientData = response.data;
        this.patient = {
          firstName: patientData.firstName,
          lastName: patientData.lastName,
          email: patientData.email,
          dateOfBirth: patientData.dateOfBirth,
          phoneNumber: patientData.phoneNumber,
          address: patientData.address
        };
      }
    });
  }

  save() {
    if (this.isEditMode && this.patientId) {
      this.patientService.update(this.patientId, this.patient).subscribe(response => {
        if (response.success) {
          alert('Patient updated successfully');
          this.router.navigate(['/patients']);
        } else {
          alert('Failed to update patient: ' + response.message);
        }
      });
    } else {
      this.patientService.create(this.patient).subscribe(response => {
        if (response.success) {
          alert('Patient created successfully');
          this.router.navigate(['/patients']);
        } else {
          alert('Failed to create patient: ' + response.message);
        }
      });
    }
  }
}