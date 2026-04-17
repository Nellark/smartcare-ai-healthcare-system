import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PatientService } from '../../../core/services/patient.service';
import { Patient } from '../../../core/models/patient.model';

@Component({
  selector: 'app-patient-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './patient-form.component.html',
  styleUrls: ['./patient-form.component.css']
})
export class PatientFormComponent implements OnInit {

  patient: Patient = {
    id: 0,
    firstName: '',
    lastName: '',
    dateOfBirth: '',
    gender: '',
    phone: '',
    email: '',
    address: ''
  };

  isEditMode = false;
  patientId: number | null = null;

  constructor(
    private patientService: PatientService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.patientId = Number(this.route.snapshot.paramMap.get('id'));

    if (this.patientId) {
      this.isEditMode = true;
      this.loadPatient(this.patientId);
    }
  }

  loadPatient(id: number) {
    this.patientService.getById(id).subscribe(data => {
      this.patient = data;
    });
  }

  save() {
    if (this.isEditMode && this.patientId) {
      this.patientService.update(this.patientId, this.patient).subscribe(() => {
        alert('Patient updated successfully');
        this.router.navigate(['/patients']);
      });
    } else {
      this.patientService.create(this.patient).subscribe(() => {
        alert('Patient created successfully');
        this.router.navigate(['/patients']);
      });
    }
  }
}