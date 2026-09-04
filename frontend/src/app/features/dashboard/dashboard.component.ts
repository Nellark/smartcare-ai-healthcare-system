import { Component, OnInit, HostListener, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { PatientService } from '../../core/services/patient.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';
import { Patient, CreatePatientRequest } from '../../core/models/patient.model';
import { DeletePatientModalComponent } from '../patients/delete-patient-modal/delete-patient-modal.component';
import { PatientModalComponent } from '../patients/patient-modal/patient-modal.component';

interface DashboardPatient {
  id: string;
  displayId: string;
  name: string;
  dateOfBirth: string;
  gender: string;
  lastVisit: string;
  status: 'Active' | 'Pending' | 'Inactive';
  avatarUrl: string;
  email?: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, DeletePatientModalComponent, PatientModalComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  private patientService = inject(PatientService);
  private authService = inject(AuthService);
  private toast = inject(ToastService);

  searchQuery = '';
  statusFilter = 'All';
  isLoading = true;
  isPatientModalVisible = false;

  currentPage = 1;
  pageSize = 10;

  // Current logged-in user info
  get currentUserEmail(): string {
    return this.authService.currentUser() || 'User';
  }

  get currentUserRole(): string {
    return this.authService.getRoleLabel(this.authService.currentUserRole());
  }

  get currentUserInitials(): string {
    const email = this.currentUserEmail;
    const parts = email.split('@')[0].split('.');
    return parts.map(p => p[0]).slice(0, 2).join('').toUpperCase();
  }

  allPatients: DashboardPatient[] = [];

  get activePatients(): number {
    return this.allPatients.filter(p => p.status === 'Active').length;
  }

  get pendingReviews(): number {
    return this.allPatients.filter(p => p.status === 'Pending').length;
  }

  get inPatients(): number {
    return this.allPatients.filter(p => p.status === 'Inactive').length;
  }

  get totalEntries(): number {
    return this.filteredPatients.length;
  }

  get filteredPatients(): DashboardPatient[] {
    return this.allPatients.filter(p => {
      const matchesSearch = !this.searchQuery ||
        p.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        p.id.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        (p.email && p.email.toLowerCase().includes(this.searchQuery.toLowerCase()));
      const matchesStatus = this.statusFilter === 'All' || p.status === this.statusFilter;
      return matchesSearch && matchesStatus;
    });
  }

  get paginatedPatients(): DashboardPatient[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredPatients.slice(start, start + this.pageSize);
  }

  get totalPages(): number {
    return Math.ceil(this.totalEntries / this.pageSize) || 1;
  }

  get displayedFrom(): number {
    if (this.totalEntries === 0) return 0;
    return (this.currentPage - 1) * this.pageSize + 1;
  }

  get displayedTo(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalEntries);
  }

  get pageNumbers(): (number | string)[] {
    const pages: (number | string)[] = [];
    const total = this.totalPages;
    if (total <= 5) {
      for (let i = 1; i <= total; i++) pages.push(i);
    } else {
      pages.push(1, 2, 3);
      if (this.currentPage > 5) pages.push('...');
      if (this.currentPage > 4 && this.currentPage < total - 2) pages.push(this.currentPage);
      pages.push('...');
      pages.push(total);
    }
    return pages;
  }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).slice(0, 2).join('').toUpperCase();
  }

  private mapPatientToDisplay(p: Patient): DashboardPatient {
    const dob = p.dateOfBirth ? new Date(p.dateOfBirth) : null;
    const dobFormatted = dob ? dob.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' }) : 'N/A';
    const lastVisit = p.updatedAt || p.createdAt;
    const lastVisitFormatted = lastVisit
      ? new Date(lastVisit).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
      : 'N/A';

    // Assign a status based on recency of last visit
    const daysSince = lastVisit
      ? (Date.now() - new Date(lastVisit).getTime()) / (1000 * 60 * 60 * 24)
      : 999;
    const status: 'Active' | 'Pending' | 'Inactive' =
      daysSince < 90 ? 'Active' : daysSince < 180 ? 'Pending' : 'Inactive';

    return {
      id: p.id,
      displayId: `SC-${p.id.slice(0, 4).toUpperCase()}`,
      name: `${p.firstName} ${p.lastName}`,
      dateOfBirth: dobFormatted,
      gender: (p as any).gender || 'N/A',
      lastVisit: lastVisitFormatted,
      status,
      avatarUrl: '',
      email: p.email
    };
  }

  ngOnInit(): void {
    this.loadPatients();
  }

  loadPatients(): void {
    this.isLoading = true;
    this.patientService.getAll().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.allPatients = response.data.map(p => this.mapPatientToDisplay(p));
        }
        this.isLoading = false;
      },
      error: (err) => {
        if (err.status === 401 || err.status === 403) {
          this.toast.error('Access Denied', 'Session expired or insufficient permissions. Please log out and sign in again.');
        } else {
          this.toast.error('Error', 'Could not load patients. Please ensure the backend is running.');
        }
        this.isLoading = false;
      }
    });
  }

  isDeleteModalVisible = false;
  patientToDelete: Patient | null = null;

  openDeleteModal(patient: DashboardPatient): void {
    // We map back to a partial Patient object since the modal only needs id, firstName, lastName
    const [firstName, ...rest] = patient.name.split(' ');
    this.patientToDelete = {
      id: patient.id,
      firstName: firstName,
      lastName: rest.join(' ')
    } as Patient;
    this.isDeleteModalVisible = true;
  }

  closeDeleteModal(): void {
    this.isDeleteModalVisible = false;
    this.patientToDelete = null;
  }

  confirmDeletePatient(event: {patientId: string, reason: string}): void {
    this.patientService.delete(event.patientId).subscribe({
      next: (response) => {
        if (response.success) {
          this.toast.success('Deleted', `The patient was deleted successfully. Reason: ${event.reason}`);
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

  openPatientModal(): void {
    this.isPatientModalVisible = true;
  }

  closePatientModal(): void {
    this.isPatientModalVisible = false;
  }

  onPatientSaved(patient: Patient): void {
    this.loadPatients();
  }


  setPage(page: number | string): void {
    if (typeof page === 'number' && page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) this.currentPage--;
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) this.currentPage++;
  }

}
