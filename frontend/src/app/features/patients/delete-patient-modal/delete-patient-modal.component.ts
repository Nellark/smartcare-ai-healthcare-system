import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ModalComponent } from '../../../shared/components/modal/modal.component';
import { Patient } from '../../../core/models/patient.model';

@Component({
  selector: 'app-delete-patient-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, ModalComponent],
  templateUrl: './delete-patient-modal.component.html',
  styleUrls: ['./delete-patient-modal.component.css']
})
export class DeletePatientModalComponent {
  @Input() isVisible = false;
  @Input() patient: Patient | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() confirmDelete = new EventEmitter<{patientId: string, reason: string}>();

  selectedReason = '';
  otherReason = '';
  isDeleting = false;

  reasons = [
    'Patient moved away',
    'Deceased',
    'Transferred to another facility',
    'Requested account deletion',
    'Duplicate record',
    'Other'
  ];

  onClose(): void {
    if (this.isDeleting) return;
    this.selectedReason = '';
    this.otherReason = '';
    this.close.emit();
  }

  onDelete(): void {
    if (!this.patient || !this.selectedReason) return;
    
    let finalReason = this.selectedReason;
    if (this.selectedReason === 'Other') {
      if (!this.otherReason.trim()) return;
      finalReason = this.otherReason.trim();
    }

    this.isDeleting = true;
    this.confirmDelete.emit({
      patientId: this.patient.id,
      reason: finalReason
    });
  }

  reset(): void {
    this.isDeleting = false;
    this.selectedReason = '';
    this.otherReason = '';
  }
}
