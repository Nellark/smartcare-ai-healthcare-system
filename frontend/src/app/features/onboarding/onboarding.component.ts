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

  constructor(private router: Router) {}

  nextStep() {
    this.step++;
  }

  completeOnboarding() {
    // Here we would typically save to the backend. 
    // For now, we'll just mock saving and redirect to dashboard.
    console.log('Onboarding complete:', this.onboardingData);
    this.router.navigate(['/app/dashboard']);
  }
}
