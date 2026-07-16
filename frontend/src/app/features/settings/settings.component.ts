import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProfileService, UserProfile } from '../../core/services/profile.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnInit {
  profileService = inject(ProfileService);
  
  isEditing = false;
  profile!: UserProfile;

  credentials = {
    licenses: [
      { type: 'Medical License', code: '#MED-209931-JS' },
      { type: 'Board Certified', code: 'Surgery', verified: true }
    ],
    education: [
      { school: 'Johns Hopkins Medicine', degree: 'Residency in General Surgery • 2018-2022' },
      { school: 'Stanford Medical School', degree: 'Doctor of Medicine (M.D.) • 2014-2018' },
      { school: 'UC Berkeley', degree: 'B.S. Molecular Biology • 2010-2014' }
    ],
    accomplishments: [
      'Top Performer 2023',
      'Clinical Research Lead',
      'ER Trauma Certified'
    ]
  };

  ngOnInit() {
    this.initWorkingCopy();
  }

  initWorkingCopy() {
    this.profile = { ...this.profileService.currentProfile() };
  }

  toggleEdit(): void {
    if (!this.isEditing) {
      this.initWorkingCopy();
      this.isEditing = true;
    }
  }

  saveProfile(): void {
    this.profileService.updateProfile(this.profile);
    this.isEditing = false;
  }

  cancelEdit(): void {
    this.initWorkingCopy();
    this.isEditing = false;
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.profile.avatar = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }

  removeAvatar(): void {
    this.profile.avatar = '';
  }
}
