import { Component, OnDestroy, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';
import { PatientService } from '../../../core/services/patient.service';
import { Patient } from '../../../core/models/patient.model';
import { ToastService } from '../../../core/services/toast.service';
import { DeletePatientModalComponent } from '../delete-patient-modal/delete-patient-modal.component';

@Component({
  selector: 'app-patient-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, DeletePatientModalComponent],
  templateUrl: './patient-list.component.html',
  styleUrls: ['./patient-list.component.css']
})
export class PatientListComponent implements OnInit, OnDestroy {

  patients = signal<Patient[]>([]);
  filteredPatients = signal<Patient[]>([]);
  isLoading = signal(false);
  searchTerm: string = '';

  // pagination signals
  currentPage = signal(1);
  pageSize = signal(10); // increased to 10 for standard tables

  paginatedPatients = computed(() => {
    const start = (this.currentPage() - 1) * this.pageSize();
    return this.filteredPatients().slice(start, start + this.pageSize());
  });

  totalPages = computed(() => {
    return Math.ceil(this.filteredPatients().length / this.pageSize()) || 1;
  });

  startIndex = computed(() => {
    if (this.filteredPatients().length === 0) return 0;
    return ((this.currentPage() - 1) * this.pageSize()) + 1;
  });

  endIndex = computed(() => {
    const end = this.currentPage() * this.pageSize();
    const total = this.filteredPatients().length;
    return end > total ? total : end;
  });

  visiblePages = computed(() => {
    const total = this.totalPages();
    const current = this.currentPage();
    const pages: (number | string)[] = [];

    if (total <= 7) {
      for (let i = 1; i <= total; i++) pages.push(i);
      return pages;
    }

    // Always show first page
    pages.push(1);

    if (current > 3) {
      pages.push('...');
    }

    const start = Math.max(2, current - 1);
    const end = Math.min(total - 1, current + 1);

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }

    if (current < total - 2) {
      pages.push('...');
    }

    // Always show last page
    pages.push(total);

    return pages;
  });

  private readonly searchTermChanged$ = new Subject<string>();
  private readonly destroy$ = new Subject<void>();

  constructor(
    private patientService: PatientService,
    private toast: ToastService
  ) {}

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

  private processPatients(data: Patient[]): Patient[] {
    return data.map(p => {
      if (!p.age && p.dateOfBirth) {
        const birthDate = new Date(p.dateOfBirth);
        const today = new Date();
        let age = today.getFullYear() - birthDate.getFullYear();
        const m = today.getMonth() - birthDate.getMonth();
        if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
          age--;
        }
        p.age = age;
      }
      return p;
    });
  }

  loadPatients() {
    this.isLoading.set(true);
    
    this.patientService.getAll().subscribe({
      next: (response) => {
        if (response.success) {
          const processed = this.processPatients(response.data);
          this.patients.set(processed);
          this.filteredPatients.set(processed);
          this.currentPage.set(1);
        } else {
          this.toast.error('Error', response.message || 'Failed to load patients');
        }
        this.isLoading.set(false);
      },
      error: (error) => {
        if (error.status === 401 || error.status === 403) {
          this.toast.error('Access Denied', 'Session expired or insufficient permissions. Please log out and sign in again.');
        } else {
          this.toast.error('Error', 'An error occurred while loading patients. Please ensure the backend API is running.');
        }
        this.isLoading.set(false);
      }
    });
  }

  onSearchTermChange() {
    this.searchTermChanged$.next(this.searchTerm);
  }

  private searchPatients(searchTerm: string) {
    const term = searchTerm.trim().toLowerCase();

    if (!term) {
      this.filteredPatients.set(this.patients());
      this.currentPage.set(1);
      return;
    }

    // Since we loaded everything, search locally for immediate feedback
    const filtered = this.patients().filter(p => 
      p.firstName.toLowerCase().includes(term) ||
      p.lastName.toLowerCase().includes(term) ||
      p.email.toLowerCase().includes(term) ||
      (p.phoneNumber && p.phoneNumber.includes(term))
    );
    this.filteredPatients.set(filtered);
    this.currentPage.set(1);
  }

  nextPage() {
    if (this.currentPage() < this.totalPages()) {
      this.currentPage.update(p => p + 1);
    }
  }

  prevPage() {
    if (this.currentPage() > 1) {
      this.currentPage.update(p => p - 1);
    }
  }

  goToPage(page: number | string) {
    if (typeof page === 'number') {
      this.currentPage.set(page);
    }
  }
  
  isDeleteModalVisible = false;
  patientToDelete: Patient | null = null;

  openDeleteModal(patient: Patient) {
    this.patientToDelete = patient;
    this.isDeleteModalVisible = true;
  }

  closeDeleteModal() {
    this.isDeleteModalVisible = false;
    this.patientToDelete = null;
  }

  confirmDeletePatient(event: {patientId: string, reason: string}) {
    this.patientService.delete(event.patientId).subscribe({
      next: (response) => {
        if (response.success) {
          this.toast.success('Patient Deleted', `The patient was deleted successfully. Reason: ${event.reason}`);
          this.loadPatients();
          this.closeDeleteModal();
        } else {
          this.toast.error('Error', response.message || 'Failed to delete patient');
        }
      },
      error: (error) => {
        let errorMessage = 'An error occurred while deleting the patient';
        if (error.error && error.error.message) {
          errorMessage = error.error.message;
        } else if (error.message) {
          errorMessage = error.message;
        }
        this.toast.error('Error', errorMessage);
      }
    });
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
