import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  private authService = inject(AuthService);

  get currentUserEmail(): string {
    return this.authService.currentUser() || '';
  }

  get currentUserRole(): string {
    return this.authService.getRoleLabel(this.authService.currentUserRole());
  }

  get isAdminUser(): boolean {
    return this.currentUserRole.toLowerCase() === 'admin';
  }

  get currentUserInitials(): string {
    const email = this.currentUserEmail;
    const namePart = email.split('@')[0];
    const parts = namePart.split('.');
    return parts.map(p => p[0]).slice(0, 2).join('').toUpperCase() || '?';
  }

  logout() {
    this.authService.logout();
  }
}
