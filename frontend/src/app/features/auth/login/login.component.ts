import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  credentials = { email: '', password: '' };
  isLoading = false;
  showPassword = false;

  private toast = inject(ToastService);

  constructor(private authService: AuthService, private router: Router) {}

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  onSubmit() {
    if (!this.credentials.email || !this.credentials.password) {
      this.toast.warning('Missing Credentials', 'Please enter both email and password');
      return;
    }

    this.isLoading = true;

    this.authService.login(this.credentials).subscribe({
      next: (res) => {
        if (!res || !res.success) {
          this.isLoading = false;
          this.toast.error('Login Failed', res?.message || 'Please verify your credentials.');
          return;
        }

        const target = this.authService.isAdminRole()
          ? '/app/admin'
          : this.authService.isPatientRole()
            ? '/app/patient-dashboard'
            : '/app/dashboard';

        this.router.navigate([target]).then(success => {
          this.isLoading = false;
          if (!success) {
            this.toast.error('Navigation Error', 'Failed to navigate to dashboard. Please try again.');
          }
        }).catch(err => {
          this.isLoading = false;
          this.toast.error('Navigation Error', err?.message || err);
        });
      },
      error: (err) => {
        this.isLoading = false;
        this.toast.error('Login Failed', err.error?.message || err.message || 'Please check your network or server.');
      }
    });
  }
}
