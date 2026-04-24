import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DashboardService, DashboardStats } from '../../core/services/dashboard.service';
import { PatientService } from '../../core/services/patient.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  dashboardStats: DashboardStats | null = null;
  isLoading = false;
  errorMessage = '';

  constructor(
    private dashboardService: DashboardService,
    private patientService: PatientService
  ) {}

  ngOnInit(): void {
    this.loadDashboardStats();
  }

  loadDashboardStats(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.patientService.getAll().subscribe({
      next: (response) => {
        if (response.success) {
          this.dashboardStats = this.dashboardService.calculateStats(response.data);
        } else {
          this.errorMessage = response.message || 'Failed to load dashboard data';
          this.dashboardStats = null;
        }
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'An error occurred while loading dashboard data';
        this.dashboardStats = null;
        this.isLoading = false;
      }
    });
  }

  refreshDashboard(): void {
    this.loadDashboardStats();
  }

  get recentPatients() {
    return this.dashboardStats?.recentPatients || [];
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    });
  }
}
