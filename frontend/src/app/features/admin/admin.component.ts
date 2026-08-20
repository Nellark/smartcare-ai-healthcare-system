import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { finalize } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardStats } from '../../core/models/dashboard.model';
import { AdminUserFormComponent } from './admin-user-form.component';
import { environment } from '../../../environments/environment';
import { ToastService } from '../../core/services/toast.service';

interface MetricCard {
  title: string;
  value: string;
  delta: string;
  icon: string;
  tone: 'blue' | 'green' | 'amber' | 'violet';
}

interface WorkstreamItem {
  label: string;
  value: string;
  status: string;
  tone: 'blue' | 'green' | 'amber';
}

interface AdminAlert {
  label: string;
  text: string;
  tone: 'critical' | 'warning' | 'info';
}

interface StatusChip {
  label: string;
  value: string;
  tone: 'good' | 'warn' | 'neutral';
}

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, RouterModule, AdminUserFormComponent],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent implements OnInit {
  private authService = inject(AuthService);
  private dashboardService = inject(DashboardService);
  private toast = inject(ToastService);
  private http = inject(HttpClient);
  private readonly healthUrl = `${environment.apiBaseUrl.replace(/\/api$/, '')}/health`;

  readonly stats: MetricCard[] = [
    {
      title: 'Patients',
      value: '0',
      delta: 'Loading live data',
      icon: 'Patients',
      tone: 'blue'
    },
    {
      title: 'Clinicians',
      value: '0',
      delta: 'Loading live data',
      icon: 'Clinics',
      tone: 'violet'
    },
    {
      title: 'Appointments',
      value: '0',
      delta: 'Loading live data',
      icon: 'Visits',
      tone: 'green'
    },
    {
      title: 'Open Issues',
      value: '0',
      delta: 'Queued from live data',
      icon: 'Alerts',
      tone: 'amber'
    }
  ];

  readonly quickActions = [
    { title: 'Review new consultations', path: '/app/new-consultation' },
    { title: 'Open patients list', path: '/app/patients' },
    { title: 'Visit analytics', path: '/app/analytics' },
    { title: 'Manage billing', path: '/app/billing' }
  ];

  alerts: AdminAlert[] = [
    {
      label: 'Loading',
      text: 'Waiting for live data before building operational alerts.',
      tone: 'info'
    }
  ];

  readonly statusChips: StatusChip[] = [
    { label: 'API', value: 'Checking', tone: 'neutral' },
    { label: 'Database', value: 'Checking', tone: 'neutral' },
    { label: 'Live sync', value: 'Pending', tone: 'neutral' }
  ];

  readonly workstreams: WorkstreamItem[] = [
    { label: 'Consultations awaiting review', value: '0', status: 'Waiting for stats', tone: 'blue' },
    { label: 'Recent registrations', value: '0', status: 'Waiting for stats', tone: 'green' },
    { label: 'Billing exceptions', value: '0', status: 'Waiting for stats', tone: 'amber' }
  ];

  readonly systemHealth = [
    { label: 'API uptime', value: '99.98%' },
    { label: 'Queue health', value: 'Healthy' },
    { label: 'Backup status', value: 'Completed' }
  ];
  isLoading = true;
  isRefreshing = false;
  recentPatients: DashboardStats['recentPatients'] = [];
  recentAppointments: DashboardStats['recentAppointments'] = [];
  lastRefreshedLabel = 'Waiting for data';
  private pendingRefreshOps = 0;

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(trackRefresh = false): void {
    this.isLoading = true;
    this.lastRefreshedLabel = this.isRefreshing ? 'Refreshing live data...' : 'Loading data...';

    if (trackRefresh) {
      this.pendingRefreshOps += 1;
    }

    this.dashboardService.getDashboardStats()
      .pipe(finalize(() => {
        this.isLoading = false;
        this.finishRefreshCycle();
      }))
      .subscribe({
        next: (response) => {
          if (!response.success || !response.data) {
            this.toast.error('Error', response.message || 'Unable to load admin dashboard data.');
            return;
          }

          this.applyStats(response.data);
        },
        error: () => {
          this.toast.error('Error', 'Unable to load admin dashboard data. Make sure the backend is running and you are authenticated.');
        }
      });

    this.loadHealth(trackRefresh);
  }

  refreshAll(): void {
    this.isRefreshing = true;
    this.pendingRefreshOps = 0;
    this.lastRefreshedLabel = 'Refreshing live data...';
    this.loadStats(true);
  }

  private applyStats(data: DashboardStats): void {
    this.recentPatients = data.recentPatients ?? [];
    this.recentAppointments = data.recentAppointments ?? [];
    this.alerts = this.buildAlerts(data);

    this.stats[0].value = data.totalPatients.toLocaleString();
    this.stats[0].delta = `${this.recentPatients.length} recent patient registrations`;

    this.stats[1].value = data.totalDoctors.toLocaleString();
    this.stats[1].delta = `${data.totalDoctors} active clinician${data.totalDoctors === 1 ? '' : 's'} on staff`;

    this.stats[2].value = data.totalAppointments.toLocaleString();
    this.stats[2].delta = `${this.recentAppointments.length} recent appointments tracked`;

    const openIssues = this.recentAppointments.filter(a => {
      const status = (a.status || '').toLowerCase();
      return status === 'scheduled' || status === 'confirmed';
    }).length;

    this.stats[3].value = openIssues.toLocaleString();
    this.stats[3].delta = openIssues > 0
      ? `${openIssues} items need follow-up`
      : 'No active issues in the current window';

    this.workstreams[0].value = this.recentAppointments.filter(a => {
      const status = (a.status || '').toLowerCase();
      return status === 'scheduled' || status === 'confirmed';
    }).length.toString();
    this.workstreams[0].status = this.recentAppointments.length
      ? `${this.recentAppointments.length} appointments in the current queue`
      : 'No appointments returned';

    this.workstreams[1].value = this.recentPatients.length.toString();
    this.workstreams[1].status = this.recentPatients.length
      ? `${this.recentPatients[0].firstName} ${this.recentPatients[0].lastName} is most recent`
      : 'No recent registrations';

    this.workstreams[2].value = openIssues.toString();
    this.workstreams[2].status = openIssues > 0
      ? 'Requires finance review'
      : 'No billing exceptions surfaced';
  }

  private buildAlerts(data: DashboardStats): AdminAlert[] {
    const alerts: AdminAlert[] = [];
    const scheduledOrConfirmed = (data.recentAppointments ?? []).filter(appointment => {
      const status = (appointment.status || '').toLowerCase();
      return status === 'scheduled' || status === 'confirmed';
    });

    if (data.totalDoctors < 3) {
      alerts.push({
        label: 'Coverage',
        text: `Only ${data.totalDoctors} clinician${data.totalDoctors === 1 ? '' : 's'} are active in the current dashboard window.`,
        tone: 'warning'
      });
    }

    if (scheduledOrConfirmed.length > 0) {
      alerts.push({
        label: 'Follow-up',
        text: `${scheduledOrConfirmed.length} appointment${scheduledOrConfirmed.length === 1 ? '' : 's'} still need review or confirmation.`,
        tone: 'critical'
      });
    }

    if ((data.recentPatients ?? []).length === 0) {
      alerts.push({
        label: 'Intake',
        text: 'No recent patient registrations were returned by the API.',
        tone: 'info'
      });
    }

    if (alerts.length === 0) {
      alerts.push({
        label: 'Stable',
        text: 'No urgent operational alerts in the current data window.',
        tone: 'info'
      });
    }

    return alerts.slice(0, 3);
  }

  private loadHealth(trackRefresh = false): void {
    if (trackRefresh) {
      this.pendingRefreshOps += 1;
    }

    this.http.get(this.healthUrl, { observe: 'response', responseType: 'text' })
      .subscribe({
        next: (response: HttpResponse<string>) => {
          const healthy = response.status >= 200 && response.status < 300;
          const body = (response.body || '').toLowerCase();
          const dbHealthy = healthy && !body.includes('unhealthy');

          this.statusChips[0].value = healthy ? 'Healthy' : 'Issue';
          this.statusChips[0].tone = healthy ? 'good' : 'warn';

          this.statusChips[1].value = dbHealthy ? 'Connected' : 'Issue';
          this.statusChips[1].tone = dbHealthy ? 'good' : 'warn';

          this.statusChips[2].value = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
          this.statusChips[2].tone = 'neutral';
          this.lastRefreshedLabel = 'Updated just now';
          this.finishRefreshCycle();
        },
        error: () => {
          this.statusChips[0].value = 'Down';
          this.statusChips[0].tone = 'warn';
          this.statusChips[1].value = 'Unknown';
          this.statusChips[1].tone = 'warn';
          this.statusChips[2].value = 'Stale';
          this.statusChips[2].tone = 'warn';
          this.lastRefreshedLabel = 'Health check unavailable';
          this.finishRefreshCycle();
        }
      });
  }

  private finishRefreshCycle(): void {
    if (this.pendingRefreshOps > 0) {
      this.pendingRefreshOps -= 1;
    }

    if (this.pendingRefreshOps === 0) {
      this.isRefreshing = false;
    }
  }

  get currentUserEmail(): string {
    return this.authService.currentUser() || 'admin@smartcare.local';
  }

  get currentUserName(): string {
    const emailName = this.currentUserEmail.split('@')[0];
    return emailName
      .split(/[.\-_]/)
      .filter(Boolean)
      .map(part => part.charAt(0).toUpperCase() + part.slice(1))
      .join(' ');
  }

  get currentUserRole(): string {
    return this.authService.getRoleLabel(this.authService.currentUserRole());
  }

  formatPatientName(patient: DashboardStats['recentPatients'][number]): string {
    return `${patient.firstName} ${patient.lastName}`.trim();
  }

  formatPatientMeta(patient: DashboardStats['recentPatients'][number]): string {
    const age = patient.age ?? 'N/A';
    return `${patient.email} · ${age}y`;
  }

  formatAppointmentMeta(appointment: DashboardStats['recentAppointments'][number]): string {
    const date = new Date(appointment.scheduledAt);
    return `${date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' })} · ${appointment.status}`;
  }
}
