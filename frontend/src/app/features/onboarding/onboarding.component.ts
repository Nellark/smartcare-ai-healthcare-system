import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-onboarding',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './onboarding.component.html',
  styleUrls: ['./onboarding.component.css']
})
export class OnboardingComponent {
  step = 1;
  isSubmitting = false;
  showSuccessModal = false;
  photoPreview: string | null = '/assets/images/doctor-avatar.png';
  appId = Math.floor(1000 + Math.random() * 9000);

  steps = ['Clinical Specialty', 'Personal Info', 'Work History', 'Review'];

  specialties = [
    { id: 'cardiology', name: 'Cardiology', desc: 'Diagnosis and treatment of heart and vascular disorders.' },
    { id: 'internal', name: 'Internal Medicine', desc: 'Comprehensive medical care for adult diseases.' },
    { id: 'neurology', name: 'Neurology', desc: 'Management of disorders affecting the brain and nerves.' },
    { id: 'surgery', name: 'General Surgery', desc: 'Surgical procedures focus on abdominal contents.' },
    { id: 'ortho', name: 'Orthopaedics', desc: 'Treatment of musculoskeletal system conditions.' },
    { id: 'ophthalmology', name: 'Ophthalmology', desc: 'Diagnosis and treatment of eye disorders.' },
    { id: 'oncology', name: 'Oncology', desc: 'Prevention, diagnosis, and treatment of cancer.' },
    { id: 'paediatrics', name: 'Paediatrics', desc: 'Medical care for infants, children, and adolescents.' },
  ];

  onboardingData = {
    specialty: '',
    firstName: '',
    lastName: '',
    title: '',
    dob: '',
    personalEmail: '',
    phone: '',
    street: '',
    city: '',
    province: '',
    postalCode: '',
    hospitalName: '',
    role: '',
    startDate: '',
    endDate: '',
    currentlyWork: false,
    responsibilities: '',
    affiliationStatus: 'Primary Admitting',
    ref1Name: '',
    ref1Contact: '',
    ref2Name: '',
    ref2Contact: '',
    certified: false
  };

  private authService = inject(AuthService);
  private toast = inject(ToastService);
  constructor(private router: Router) {}

  saveDraft() {
    this.toast.info('Draft Saved', 'Your onboarding progress has been saved as draft.');
  }

  onPhotoChange(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = () => { this.photoPreview = reader.result as string; };
    reader.readAsDataURL(file);
  }

  nextStep() {
    if (this.step < 4) this.step++;
  }

  completeOnboarding() {
    this.isSubmitting = true;
    setTimeout(() => {
      this.isSubmitting = false;
      this.showSuccessModal = true;
    }, 900);
  }

  goToDashboard() {
    if (this.authService.isPatientRole()) {
      this.router.navigate(['/app/patient-dashboard']);
    } else {
      this.router.navigate(['/app/dashboard']);
    }
  }
}
