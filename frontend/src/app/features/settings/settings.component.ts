import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
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
