import { Component, inject } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private toast = inject(ToastService);
  
  isLoadingDemo = false;

  startDemo() {
    this.isLoadingDemo = true;
    this.toast.info('Starting Demo', 'Logging in to the demo environment...');
    
    this.authService.login({ email: 'admin@smartcare.local', password: 'Admin@123' }).subscribe({
      next: (res) => {
        this.isLoadingDemo = false;
        if (res && res.success) {
          const target = this.authService.isAdminRole()
            ? '/app/admin'
            : this.authService.isPatientRole()
              ? '/app/patient-dashboard'
              : '/app/dashboard';
          this.router.navigate([target]);
        } else {
          this.toast.error('Demo Login Failed', 'Could not start the demo at this time.');
        }
      },
      error: (err) => {
        this.isLoadingDemo = false;
        this.toast.error('Demo Login Failed', 'Please ensure the backend is running.');
      }
    });
  }
}
