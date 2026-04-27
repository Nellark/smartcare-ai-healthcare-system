import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';
import { PatientService } from '../../../core/services/patient.service';
import { Patient } from '../../../core/models/patient.model';

@Component({
  selector: 'app-patient-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './patient-list.component.html',
  styleUrls: ['./patient-list.component.css']
})
export class PatientListComponent implements OnInit, OnDestroy {

  patients: Patient[] = [];
  filteredPatients: Patient[] = [];
  isLoading = false;
  errorMessage = '';

  searchTerm: string = '';

  // pagination
  currentPage = 1;
  pageSize = 5;
  private readonly searchTermChanged$ = new Subject<string>();
  private readonly destroy$ = new Subject<void>();

  constructor(private patientService: PatientService) {}

  ngOnInit(): void {
    this.loadPatients();
    this.searchTermChanged$
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntil(this.destroy$)
      )
      .subscribe((term) => {
        this.searchPatients(term);
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.searchTermChanged$.complete();
  }

  loadPatients() {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.patientService.getAll().subscribe({
      next: (response) => {
        if (response.success) {
          this.patients = response.data;
          this.filteredPatients = response.data;
          this.currentPage = 1;
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

  onSearchTermChange() {
    this.searchTermChanged$.next(this.searchTerm);
  }

  private searchPatients(searchTerm: string) {
    const term = searchTerm.trim();

    if (!term) {
      this.loadPatients();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.patientService.search(term).subscribe({
      next: (response) => {
        if (response.success) {
          this.patients = response.data;
          this.filteredPatients = response.data;
          this.currentPage = 1;
        } else {
          this.errorMessage = response.message || 'Failed to search patients';
        }
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'An error occurred while searching patients';
        this.isLoading = false;
      }
    });
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
            this.searchPatients(this.searchTerm);
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
