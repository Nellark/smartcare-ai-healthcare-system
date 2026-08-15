import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  credentials = { email: '', password: '' };
  errorMessage = '';
  isLoading = false;

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit() {
    if (!this.credentials.email || !this.credentials.password) {
      this.errorMessage = 'Please enter both email and password';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login(this.credentials).subscribe({
      next: (res) => {
        if (!res || !res.success) {
          this.isLoading = false;
          this.errorMessage = res?.message || 'Login failed. Please verify your credentials.';
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
            this.errorMessage = 'Failed to navigate to dashboard. Please try again.';
          }
        }).catch(err => {
          this.isLoading = false;
          this.errorMessage = 'Navigation error: ' + (err?.message || err);
        });
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || err.message || 'Login failed. Please check your network or server.';
      }
    });
  }
}
