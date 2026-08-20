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
      <div class="field-grid">
        <div class="field">
          <label for="admin-email">Email</label>
          <input id="admin-email" type="email" [(ngModel)]="email" name="email" placeholder="name@clinic.com" required />
        </div>

        <div class="field">
          <label for="admin-password">Temporary password</label>
          <input id="admin-password" type="password" [(ngModel)]="password" name="password" placeholder="Set a secure starter password" required />
        </div>
      </div>

      <div class="field">
        <label for="admin-role">Role</label>
        <select id="admin-role" [(ngModel)]="role" name="role">
          <option value="Patient">Patient</option>
          <option value="Doctor">Provider</option>
          <option value="Admin">Admin</option>
        </select>
      </div>

      <div class="form-actions">
        <button type="submit" [disabled]="isSubmitting">Create user</button>
        <p class="hint">Use this for staff onboarding or demo accounts.</p>
      </div>

      <p class="message" [class.error]="messageType === 'error'" [class.success]="messageType === 'success'" *ngIf="message">
        {{ message }}
      </p>
    </form>
  `,
  styles: [
    `.admin-form{display:flex;flex-direction:column;gap:1rem;max-width:100%;}`,
    `.field-grid{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:0.9rem;}`,
    `.field{display:flex;flex-direction:column;gap:0.4rem;}`,
    `label{font-size:0.84rem;font-weight:700;color:#475569;}`,
    `input,select{padding:0.85rem 0.95rem;border:1px solid #cbd5e1;border-radius:14px;background:#fff;transition:border-color 150ms ease, box-shadow 150ms ease;}`,
    `input:focus,select:focus{outline:none;border-color:#026c7c;box-shadow:0 0 0 3px rgba(2,108,124,0.1);}`,
    `input::placeholder{color:#94a3b8;}`,
    `.form-actions{display:flex;flex-wrap:wrap;align-items:center;gap:0.75rem;justify-content:space-between;}`,
    `button{padding:0.85rem 1.05rem;border:0;border-radius:999px;background:linear-gradient(135deg,#026c7c,#0f766e);color:white;cursor:pointer;font-weight:700;box-shadow:0 10px 24px rgba(2,108,124,0.2);}`,
    `button:disabled{opacity:0.65;cursor:not-allowed;}`,
    `.hint{margin:0;color:#64748b;font-size:0.85rem;}`,
    `.message{margin:0;font-size:0.92rem;padding:0.8rem 0.9rem;border-radius:12px;background:#f8fafc;border:1px solid #e2e8f0;color:#0f172a;}`,
    `.message.success{background:#ecfdf5;border-color:#a7f3d0;color:#047857;}`,
    `.message.error{background:#fef2f2;border-color:#fecaca;color:#b91c1c;}`,
    `@media (max-width: 720px){.field-grid{grid-template-columns:1fr;}.form-actions{align-items:stretch;}.form-actions button{width:100%;}}`
  ]
})
export class AdminUserFormComponent {
  private authService = inject(AuthService);

  email = '';
  password = '';
  role = 'Patient';
  message = '';
  messageType: 'success' | 'error' | '' = '';
  isSubmitting = false;

  createUser() {
    this.message = '';
    this.messageType = '';
    this.isSubmitting = true;
    this.authService.createUserByAdmin({ email: this.email, password: this.password, role: this.role }).subscribe({
      next: () => {
        this.messageType = 'success';
        this.message = 'User created successfully.';
        this.email = '';
        this.password = '';
        this.role = 'Patient';
        this.isSubmitting = false;
      },
      error: (err) => {
        this.messageType = 'error';
        this.message = err.error?.message || 'Unable to create user.';
        this.isSubmitting = false;
      }
    });
  }
}
