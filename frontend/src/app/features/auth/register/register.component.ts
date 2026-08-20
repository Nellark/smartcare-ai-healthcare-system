import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

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

  private toast = inject(ToastService);

  setRole(role: string) {
    this.userData.role = role;
  }

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit() {
    if (!this.userData.email || !this.userData.password) {
      this.toast.warning('Missing Credentials', 'Please enter both email and password');
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
      next: () => {
        this.isLoading = false;
        if (this.authService.isAdminRole()) {
          this.router.navigate(['/app/admin']);
        } else if (this.authService.isPatientRole()) {
          this.router.navigate(['/app/patient-dashboard']);
        } else {
          this.router.navigate(['/app/dashboard']);
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.toast.error('Registration Failed', err.error?.message || 'Registration failed');
      }
    });
  }
}

