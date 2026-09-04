import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { MockDataService } from '../../core/services/mock-data.service';

interface Vital {
  label: string;
  value: string;
  unit: string;
}

interface Prescription {
  name: string;
  dosage: string;
  frequency: string;
}

interface Activity {
  type: string;
  description: string;
  timeAgo: string;
  color: 'blue' | 'gold' | 'gray';
  actionLabel?: string;
}

interface Resource {
  category: string;
  title: string;
  description: string;
  imageUrl: string;
}

@Component({
  selector: 'app-patient-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './patient-dashboard.component.html',
  styleUrls: ['./patient-dashboard.component.css']
})
export class PatientDashboardComponent implements OnInit {
  private authService = inject(AuthService);
  private mockDataService = inject(MockDataService);

  get currentUserEmail(): string {
    return this.authService.currentUser() || '';
  }

  get currentUserName(): string {
    const email = this.currentUserEmail;
    const namePart = email.split('@')[0];
    return namePart.split('.').map(p => p.charAt(0).toUpperCase() + p.slice(1)).join(' ');
  }

  get greeting(): string {
    return 'GOOD MORNING';
  }

  get todayFormatted(): string {
    return new Date().toLocaleDateString('en-US', {
      month: 'long', day: 'numeric', year: 'numeric'
    });
  }

  // --- Mock Data Loaded Dynamically ---
  vitals: Vital[] = [];
  prescriptions: Prescription[] = [];
  nextAppointment = {
    doctor: '',
    title: '',
    date: '',
    location: ''
  };
  recentActivity: Activity[] = [];
  activityProgress = {
    stepGoal: '',
    calories: '',
    activeMin: ''
  };
  resources: Resource[] = [];

  ngOnInit(): void {
    this.mockDataService.getPatientDashboardData().subscribe({
      next: (data) => {
        this.vitals = data.vitals;
        this.prescriptions = data.prescriptions;
        this.nextAppointment = data.nextAppointment;
        this.recentActivity = data.recentActivity;
        this.activityProgress = data.activityProgress;
        this.resources = data.resources;
      }
    });
  }
}
