import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  userData = { fullName: '', hospitalName: '', email: '', license: '', password: '', role: 'Patient' };
  errorMessage = '';
  isLoading = false;

  setRole(role: string) {
    this.userData.role = role;
  }

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit() {
    if (!this.userData.email || !this.userData.password) {
      this.errorMessage = 'Please enter both email and password';
      return;
    }
    
    // Additional validation could be added for fullName, hospitalName, etc.

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.register({ 
      email: this.userData.email, 
      password: this.userData.password,
      role: this.userData.role 
    }).subscribe({
      next: () => {
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
        this.errorMessage = err.error?.message || 'Registration failed';
      }
    });
  }
}
