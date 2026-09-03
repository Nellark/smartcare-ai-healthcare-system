import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-onboarding',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './onboarding.component.html',
  styleUrls: ['./onboarding.component.css']
})
export class OnboardingComponent {
  step = 1;
  onboardingData = {
    clinicName: '',
    specialization: '',
    fullName: ''
  };

  isSubmitting = false;
  showSuccessModal = false;

  private authService = inject(AuthService);

  constructor(private router: Router) {}

  nextStep() {
    this.step++;
  }

  completeOnboarding() {
    this.isSubmitting = true;
    setTimeout(() => {
      this.isSubmitting = false;
      this.showSuccessModal = true;
    }, 800);
  }

  goToDashboard() {
    // Route based on role — Patients go to patient dashboard, everyone else to provider dashboard
    if (this.authService.isPatientRole()) {
      this.router.navigate(['/app/patient-dashboard']);
    } else {
      this.router.navigate(['/app/dashboard']);
    }
  }
}
