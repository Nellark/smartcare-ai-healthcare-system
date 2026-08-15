import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AdminUserFormComponent } from './admin-user-form.component';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, RouterModule, AdminUserFormComponent],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent {
  readonly stats = [
    {
      title: 'Patients',
      value: '1,248',
      change: '+12% this month',
      icon: '👥',
      tone: 'blue'
    },
    {
      title: 'Appointments',
      value: '386',
      change: '18 pending review',
      icon: '🗓️',
      tone: 'violet'
    },
    {
      title: 'Revenue',
      value: '$84.2k',
      change: '+8.4% vs last week',
      icon: '💳',
      tone: 'green'
    },
    {
      title: 'Open Issues',
      value: '7',
      change: '3 critical alerts',
      icon: '⚠️',
      tone: 'orange'
    }
  ];

  readonly quickActions = [
    { title: 'Review new consultations', path: '/app/new-consultation' },
    { title: 'Open patients list', path: '/app/patients' },
    { title: 'Visit analytics', path: '/app/analytics' },
    { title: 'Manage billing', path: '/app/billing' }
  ];

  readonly alerts = [
    'Medication stock for insulin is below threshold.',
    'Two staff schedules need approval before tomorrow.',
    'Lab results for 4 patients are ready for review.'
  ];

  readonly systemHealth = [
    { label: 'API uptime', value: '99.98%' },
    { label: 'Queue health', value: 'Healthy' },
    { label: 'Backup status', value: 'Completed' }
  ];
}
