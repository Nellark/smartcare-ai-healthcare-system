import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { PatientService } from '../../../core/services/patient.service';
import { Patient } from '../../../core/models/patient.model';

@Component({
  selector: 'app-patient-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './patient-list.component.html',
  styleUrls: ['./patient-list.component.css']
})
export class PatientListComponent implements OnInit {

  patients: Patient[] = [];
  filteredPatients: Patient[] = [];
  isLoading = false;
  errorMessage = '';

  searchTerm: string = '';

  // pagination
  currentPage = 1;
  pageSize = 5;

  constructor(private patientService: PatientService) {}

  ngOnInit(): void {
    this.loadPatients();
  }

  loadPatients() {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.patientService.getAll().subscribe({
      next: (response) => {
        if (response.success) {
          this.patients = response.data;
          this.applyFilter();
        } else {
          this.errorMessage = response.message || 'Failed to load patients';
        }
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'An error occurred while loading patients';
        this.isLoading = false;
      }
    });
  }

  applyFilter() {
    const term = this.searchTerm.toLowerCase();

    this.filteredPatients = this.patients.filter(p =>
      `${p.firstName} ${p.lastName}`.toLowerCase().includes(term) ||
      p.email.toLowerCase().includes(term) ||
      p.phoneNumber.includes(term)
    );

    this.currentPage = 1; // reset page on search
  }

  get paginatedPatients() {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredPatients.slice(start, start + this.pageSize);
  }

  totalPages(): number {
    return Math.ceil(this.filteredPatients.length / this.pageSize);
  }

  nextPage() {
    if (this.currentPage < this.totalPages()) {
      this.currentPage++;
    }
  }

  prevPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  
  deletePatient(id: string) {
    if (confirm('Are you sure you want to delete this patient?')) {
      this.patientService.delete(id).subscribe({
        next: (response) => {
          if (response.success) {
            this.loadPatients();
          } else {
            this.errorMessage = response.message || 'Failed to delete patient';
          }
        },
        error: (error) => {
          this.errorMessage = 'An error occurred while deleting the patient';
        }
      });
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
}