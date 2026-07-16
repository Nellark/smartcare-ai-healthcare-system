import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

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
export class PatientDashboardComponent {
  private authService = inject(AuthService);

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

  // --- Mock Data ---

  vitals: Vital[] = [
    { label: 'Blood Pressure', value: '118/76', unit: 'mmHg' },
    { label: 'Heart Rate', value: '72', unit: 'bpm' }
  ];

  prescriptions: Prescription[] = [
    { name: 'Lisinopril', dosage: '10mg', frequency: 'Once daily, morning' },
    { name: 'Atorvastatin', dosage: '20mg', frequency: 'Once daily, evening' }
  ];

  nextAppointment = {
    doctor: 'Dr. Sarah Richardson',
    title: 'Annual Comprehensive Wellness Review',
    date: 'June 22, 10:30 AM',
    location: 'Main Campus, Wing B'
  };

  recentActivity: Activity[] = [
    { type: 'Lab results are ready for review', description: 'BLOOD PANEL', timeAgo: '45M AGO', color: 'blue', actionLabel: 'VIEW REPORT' },
    { type: 'Prescription renewed', description: 'LISINOPRIL', timeAgo: '4H AGO', color: 'gold' },
    { type: 'Message from Dr. Richardson', description: 'PATIENT PORTAL', timeAgo: 'YESTERDAY', color: 'gray', actionLabel: 'REPLY TO MESSAGE' }
  ];

  activityProgress = {
    stepGoal: '8,420',
    calories: '420',
    activeMin: '45'
  };

  resources: Resource[] = [
    {
      category: 'NUTRITION',
      title: 'The Gut-Heart Connection: Recent Findings',
      description: 'New clinical studies suggest that microbiome health plays a critical role in cardiovascular...',
      imageUrl: 'https://images.unsplash.com/photo-1512621776951-a57141f2eefd?q=80&w=800&auto=format&fit=crop'
    },
    {
      category: 'MENTAL WELLNESS',
      title: 'Cognitive Reserve and Aging',
      description: 'Understanding how mental stimulation builds resilience against cognitive decline in later...',
      imageUrl: 'https://images.unsplash.com/photo-1559757175-5700dde675bc?q=80&w=800&auto=format&fit=crop'
    },
    {
      category: 'TECHNOLOGY',
      title: 'Integrating Wearable Data into Your Care',
      description: 'Learn how to securely share your daily activity metrics with your SmartCare provider team.',
      imageUrl: 'https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?q=80&w=800&auto=format&fit=crop'
    }
  ];
}
