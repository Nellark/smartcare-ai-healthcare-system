import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { ProfileService } from '../../core/services/profile.service';

interface UserSettings {
  fullName: string;
  email: string;
  phone: string;
  altPhone: string;
  address: string;
  twoFactorEnabled: boolean;
  passwordLastChanged: string;
  emailNotifications: boolean;
  smsNotifications: boolean;
  resultsPortalSync: boolean;
  weeklyHealthDigest: boolean;
}

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnInit {
  private profileService = inject(ProfileService);
  private authService = inject(AuthService);

  profile: UserSettings = {
    fullName: '',
    email: '',
    phone: '',
    altPhone: '',
    address: '',
    twoFactorEnabled: true,
    passwordLastChanged: 'Last changed 4 months ago',
    emailNotifications: true,
    smsNotifications: false,
    resultsPortalSync: true,
    weeklyHealthDigest: false
  };

  isSaving = false;
  saveSuccess = false;
  showPasswordForm = false;
  isChangingPassword = false;
  passwordChangeError = '';
  passwordChangeSuccess = false;

  passwordForm = {
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  };

  ngOnInit() {
    const userProfile = this.profileService.currentProfile();
    this.profile.fullName = userProfile.fullName || userProfile.name;
    this.profile.email = userProfile.email;
    this.profile.phone = userProfile.contact !== 'Not provided' ? userProfile.contact : '+27 (82) 123-4567';
    this.profile.altPhone = '+27 (71) 987-6543'; // Default fallback
    this.profile.address = userProfile.office !== 'Not provided' ? userProfile.office : 'Block B, Nelson Mandela Square, Sandton, 2196';
  }

  saveProfile() {
    this.isSaving = true;
    this.saveSuccess = false;

    // Simulate API call
    setTimeout(() => {
      this.profileService.updateProfile({
        fullName: this.profile.fullName,
        name: this.profile.fullName,
        email: this.profile.email,
        contact: this.profile.phone,
        office: this.profile.address
      });
      
      this.isSaving = false;
      this.saveSuccess = true;
      
      // Reset success message after 3 seconds
      setTimeout(() => {
        this.saveSuccess = false;
      }, 3000);
    }, 800);
  }

  togglePasswordChangePanel() {
    this.showPasswordForm = !this.showPasswordForm;
    if (!this.showPasswordForm) {
      this.passwordForm = {
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
      };
    }
    this.passwordChangeError = '';
    this.passwordChangeSuccess = false;
  }

  validatePasswordChange(): string | null {
    if (!this.passwordForm.currentPassword.trim()) {
      return 'Current password is required.';
    }

    if (!this.passwordForm.newPassword.trim()) {
      return 'New password is required.';
    }

    if (this.passwordForm.newPassword !== this.passwordForm.confirmPassword) {
      return 'New password and confirmation must match.';
    }

    const passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,128}$/;
    if (!passwordPattern.test(this.passwordForm.newPassword)) {
      return 'Use at least 8 characters with uppercase, lowercase, a number, and a special character.';
    }

    if (this.passwordForm.currentPassword === this.passwordForm.newPassword) {
      return 'New password must be different from your current password.';
    }

    return null;
  }

  changePassword() {
    const validationMessage = this.validatePasswordChange();
    if (validationMessage) {
      this.passwordChangeError = validationMessage;
      this.passwordChangeSuccess = false;
      return;
    }

    this.isChangingPassword = true;
    this.passwordChangeError = '';

    this.authService.changePassword({
      currentPassword: this.passwordForm.currentPassword,
      newPassword: this.passwordForm.newPassword
    }).subscribe({
      next: (response) => {
        this.isChangingPassword = false;
        this.passwordChangeSuccess = true;
        this.profile.passwordLastChanged = 'Last changed just now';
        this.passwordForm = {
          currentPassword: '',
          newPassword: '',
          confirmPassword: ''
        };
        this.showPasswordForm = false;
      },
      error: (error) => {
        this.isChangingPassword = false;
        this.passwordChangeSuccess = false;
        this.passwordChangeError = error?.error?.message || 'Unable to update password right now. Please try again.';
      }
    });
  }

  toggle2FA() {
    this.profile.twoFactorEnabled = !this.profile.twoFactorEnabled;
  }

  toggleEmailNotifications() {
    this.profile.emailNotifications = !this.profile.emailNotifications;
  }

  toggleSmsNotifications() {
    this.profile.smsNotifications = !this.profile.smsNotifications;
  }

  toggleResultsPortalSync() {
    this.profile.resultsPortalSync = !this.profile.resultsPortalSync;
  }

  toggleWeeklyHealthDigest() {
    this.profile.weeklyHealthDigest = !this.profile.weeklyHealthDigest;
  }
}
