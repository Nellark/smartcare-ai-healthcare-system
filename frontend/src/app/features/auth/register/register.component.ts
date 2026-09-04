import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

interface PasswordRequirement {
  label: string;
  met: boolean;
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  userData = { fullName: '', hospitalName: '', email: '', license: '', password: '', role: 'Patient' };
  isLoading = false;
  showPassword = false;
  passwordRequirements: PasswordRequirement[] = [];

  private toast = inject(ToastService);

  constructor(private authService: AuthService, private router: Router) {}

  setRole(role: string) {
    this.userData.role = role;
  }

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  get passwordErrors(): string[] {
    const p = this.userData.password;
    const errors: string[] = [];
    if (!p) return errors;
    if (p.length < 8) errors.push('At least 8 characters');
    if (!/[A-Z]/.test(p)) errors.push('One uppercase letter');
    if (!/[a-z]/.test(p)) errors.push('One lowercase letter');
    if (!/\d/.test(p)) errors.push('One number');
    if (!/[!@#$%^&*()\-_=+\[\]{}|;':",./<>?\\]/.test(p)) errors.push('One special character (!@#$%^&*)');
    return errors;
  }

  get passwordStrength(): { level: 'weak' | 'fair' | 'strong' | 'none'; label: string } {
    const p = this.userData.password;
    if (!p) return { level: 'none', label: '' };
    const score = [
      p.length >= 8,
      p.length >= 12,
      /[A-Z]/.test(p),
      /[a-z]/.test(p),
      /\d/.test(p),
      /[!@#$%^&*()\-_=+\[\]{}|;':",./<>?\\]/.test(p)
    ].filter(Boolean).length;

    if (score <= 2) return { level: 'weak', label: 'Weak' };
    if (score <= 4) return { level: 'fair', label: 'Fair' };
    return { level: 'strong', label: 'Strong' };
  }

  get passwordRequirementsList(): PasswordRequirement[] {
    const p = this.userData.password;
    return [
      { label: 'At least 8 characters', met: p.length >= 8 },
      { label: 'One uppercase letter (A–Z)', met: /[A-Z]/.test(p) },
      { label: 'One lowercase letter (a–z)', met: /[a-z]/.test(p) },
      { label: 'One number (0–9)', met: /\d/.test(p) },
      { label: 'One special character (!@#$%^&*)', met: /[!@#$%^&*()\-_=+\[\]{}|;':",./<>?\\]/.test(p) },
    ];
  }

  onSubmit() {
    if (!this.userData.email || !this.userData.password) {
      this.toast.warning('Missing Credentials', 'Please enter both email and password');
      return;
    }

    // Block submission if password requirements not met
    if (this.passwordErrors.length > 0) {
      this.toast.error(
        'Password Too Weak',
        'Missing: ' + this.passwordErrors.join(', ') + '.'
      );
      return;
    }

    // Prevent self-assigning Admin role via the public registration form
    const role = this.userData.role === 'Admin' ? 'Patient' : this.userData.role;

    this.isLoading = true;

    this.authService.register({
      email: this.userData.email,
      password: this.userData.password,
      role
    }).subscribe({
      next: (res) => {
        this.isLoading = false;

        if (!res || !res.success) {
          this.toast.error('Registration Failed', res?.message || 'Registration failed. Please try again.');
          return;
        }

        // Use role from response directly — the signal may not be updated yet
        const returnedRole = (res.data as any)?.Role || (res.data as any)?.role || role;
        const normalizedRole = returnedRole?.toLowerCase();

        if (normalizedRole === 'admin') {
          this.router.navigate(['/app/admin']);
        } else {
          // All new users (Patient, Doctor, Nurse) go through onboarding first
          this.router.navigate(['/onboarding']);
        }
      },
      error: (err) => {
        this.isLoading = false;
        // Surface the detailed error list from the backend if available
        const errors: string[] = err.error?.errors || [];
        const message = errors.length > 0
          ? errors.join(' ')
          : (err.error?.message || 'Registration failed. Please try again.');
        this.toast.error('Registration Failed', message);
      }
    });
  }
}
