import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-admin-user-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <form class="admin-form" (ngSubmit)="createUser()">
      <div class="field">
        <label>Email</label>
        <input type="email" [(ngModel)]="email" name="email" required />
      </div>
      <div class="field">
        <label>Password</label>
        <input type="password" [(ngModel)]="password" name="password" required />
      </div>
      <div class="field">
        <label>Role</label>
        <select [(ngModel)]="role" name="role">
          <option value="Patient">Patient</option>
          <option value="Doctor">Provider</option>
          <option value="Admin">Admin</option>
        </select>
      </div>
      <button type="submit">Create user</button>
      <p class="message" *ngIf="message">{{ message }}</p>
    </form>
  `,
  styles: [
    `.admin-form{display:flex;flex-direction:column;gap:0.8rem;max-width:420px;}`,
    `.field{display:flex;flex-direction:column;gap:0.35rem;}`,
    `input,select,button{padding:0.7rem;border:1px solid #cbd5e1;border-radius:10px;}`,
    `button{background:#2563eb;color:white;cursor:pointer;font-weight:600;}`,
    `.message{color:#0f766e;font-size:0.95rem;}`
  ]
})
export class AdminUserFormComponent {
  private authService = inject(AuthService);

  email = '';
  password = '';
  role = 'Patient';
  message = '';

  createUser() {
    this.message = '';
    this.authService.createUserByAdmin({ email: this.email, password: this.password, role: this.role }).subscribe({
      next: () => {
        this.message = 'User created successfully.';
        this.email = '';
        this.password = '';
        this.role = 'Patient';
      },
      error: (err) => {
        this.message = err.error?.message || 'Unable to create user.';
      }
    });
  }
}
