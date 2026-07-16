import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

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

  constructor(private router: Router) {}

  nextStep() {
    this.step++;
  }

  completeOnboarding() {
    this.isSubmitting = true;
    // Simulate API call
    setTimeout(() => {
      this.isSubmitting = false;
      this.showSuccessModal = true;
    }, 800);
  }
  
  goToDashboard() {
    this.router.navigate(['/app/dashboard']);
  }
}
