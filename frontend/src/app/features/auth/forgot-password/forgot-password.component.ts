import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService, PasswordResetLink } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

interface PasswordRequirement {
  label: string;
  met: boolean;
}

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['../login/login.component.css', './forgot-password.component.css']
})
export class ForgotPasswordComponent implements OnInit {
  // Stage 1 fields
  email = '';

  // Stage 2 fields
  token = '';
  newPassword = '';

  isLoading = false;
  stage: 1 | 2 = 1;
  showPassword = false;

  private toast = inject(ToastService);

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const emailFromQuery = this.route.snapshot.queryParamMap.get('email') ?? '';
    const tokenFromQuery = this.route.snapshot.queryParamMap.get('token') ?? '';

    if (emailFromQuery) {
      this.email = emailFromQuery;
    }

    if (tokenFromQuery) {
      this.token = tokenFromQuery;
      this.stage = 2;
      this.toast.success('Reset link loaded', 'Please create a new password to continue.');
    }
  }

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  getResetLink(): string {
    const link = this.route.snapshot.queryParamMap.get('resetLink');
    if (link) {
      return new URL(link, window.location.origin).toString();
    }

    return `${window.location.origin}/forgot-password?email=${encodeURIComponent(this.email)}&token=${encodeURIComponent(this.token)}`;
  }

  openResetLink(): void {
    if (!this.token) {
      this.toast.warning('No reset link', 'Generate a reset link before opening it.');
      return;
    }

    window.open(this.getResetLink(), '_blank', 'noopener,noreferrer');
  }

  get passwordErrors(): string[] {
    const p = this.newPassword;
    const errors: string[] = [];
    if (!p) return errors;
    if (p.length < 8) errors.push('At least 8 characters');
    if (!/[A-Z]/.test(p)) errors.push('One uppercase letter');
    if (!/[a-z]/.test(p)) errors.push('One lowercase letter');
    if (!/\d/.test(p)) errors.push('One number');
    if (!/[!@#$%^&*()\-_=+\[\]{}|;':",.\/<>?\\]/.test(p)) errors.push('One special character (!@#$%^&*)');
    return errors;
  }

  get passwordStrength(): { level: 'weak' | 'fair' | 'strong' | 'none'; label: string } {
    const p = this.newPassword;
    if (!p) return { level: 'none', label: '' };
    const score = [
      p.length >= 8,
      p.length >= 12,
      /[A-Z]/.test(p),
      /[a-z]/.test(p),
      /\d/.test(p),
      /[!@#$%^&*()\-_=+\[\]{}|;':",.\/<>?\\]/.test(p)
    ].filter(Boolean).length;
    if (score <= 2) return { level: 'weak', label: 'Weak' };
    if (score <= 4) return { level: 'fair', label: 'Fair' };
    return { level: 'strong', label: 'Strong' };
  }

  get passwordRequirementsList(): PasswordRequirement[] {
    const p = this.newPassword;
    return [
      { label: 'At least 8 characters', met: p.length >= 8 },
      { label: 'One uppercase letter (A–Z)', met: /[A-Z]/.test(p) },
      { label: 'One lowercase letter (a–z)', met: /[a-z]/.test(p) },
      { label: 'One number (0–9)', met: /\d/.test(p) },
      { label: 'One special character (!@#$%^&*)', met: /[!@#$%^&*()\-_=+\[\]{}|;':",.\/<>?\\]/.test(p) },
    ];
  }

  onSubmitRequest() {
    if (!this.email) {
      this.toast.warning('Email Required', 'Please enter your email address');
      return;
    }

    this.isLoading = true;
    this.authService.forgotPassword(this.email).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res && res.success) {
          const payload = res.data as PasswordResetLink | string | null;
          const resetData = typeof payload === 'string' ? { email: this.email, token: '', resetLink: '' } : payload;

          if (resetData?.email) {
            this.email = resetData.email;
          }

          this.stage = 1;
          this.toast.success('Reset link sent', res.message || 'Check your email for the secure reset link.');
        } else {
          this.toast.error('Error', res?.message || 'Failed to request reset');
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.toast.error('Error', err.error?.message || 'Failed to request password reset');
      }
    });
  }

  onSubmitReset() {
    if (!this.token || !this.newPassword) {
      this.toast.warning('Incomplete', 'Please provide a new password and token');
      return;
    }

    if (this.passwordErrors.length > 0) {
      this.toast.error('Weak Password', 'Please meet all password requirements before continuing.');
      return;
    }

    this.isLoading = true;
    this.authService.resetPassword({ email: this.email, token: this.token, newPassword: this.newPassword }).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res && res.success) {
          this.toast.success('Success', 'Password reset successfully');
          this.router.navigate(['/login']);
        } else {
          this.toast.error('Error', res?.message || 'Failed to reset password');
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.toast.error('Reset Failed', err.error?.message || 'Failed to reset password');
      }
    });
  }
}
