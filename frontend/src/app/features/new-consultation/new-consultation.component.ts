import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ConsultationStoreService } from '../../core/services/consultation-store.service';
import { MockDataService } from '../../core/services/mock-data.service';
import { DoctorService } from '../../core/services/doctor.service';

interface Provider {
  id: string;
  name: string;
  specialty: string;
  rating: number;
  reviews: number;
  bio: string;
  avatarUrl: string;
  selected: boolean;
}

interface CalendarDay {
  date: number;
  fullDate: Date;
  isCurrentMonth: boolean;
  isToday: boolean;
  isPast: boolean;
}

@Component({
  selector: 'app-new-consultation',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './new-consultation.component.html',
  styleUrls: ['./new-consultation.component.css']
})
export class NewConsultationComponent implements OnInit {
  private router = inject(Router);
  private store = inject(ConsultationStoreService);
  private mockDataService = inject(MockDataService);
  private doctorService = inject(DoctorService);

  constructor() {
    this.buildCalendar();
  }

  currentStep = 1;
  searchQuery = '';
  selectedReason = '';
  notes = '';

  reasons: string[] = [];
  providers: Provider[] = [];

  get selectedProvider(): Provider | undefined {
    return this.providers.find(p => p.selected);
  }

  selectProvider(provider: Provider): void {
    this.providers.forEach(p => p.selected = false);
    provider.selected = true;
  }

  ngOnInit(): void {
    this.mockDataService.getNewConsultationData().subscribe({
      next: (data) => {
        this.reasons = data.reasons;
        if (data.reasons.length > 0) {
          this.selectedReason = data.reasons[0];
        }
        this.timeSlots = data.timeSlots;
        const availableSlot = data.timeSlots.find((s: any) => s.available);
        if (availableSlot) {
          this.selectedTime = availableSlot.label;
        }
      }
    });

    this.doctorService.getAll().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.providers = response.data.map(d => ({
            id: d.id,
            name: `Dr. ${d.firstName} ${d.lastName}`,
            specialty: d.specialty,
            rating: 4.8,
            reviews: 124,
            bio: `Specialist in ${d.specialty} with years of clinical experience. Dedicated to providing personalized patient care.`,
            avatarUrl: `https://ui-avatars.com/api/?name=${d.firstName}+${d.lastName}&background=026C7C&color=fff`,
            selected: false
          }));
        }
      }
    });
  }

  // ─── Real Calendar Logic ───────────────────────────────────────
  today = new Date();
  viewDate = new Date(this.today.getFullYear(), this.today.getMonth(), 1);
  selectedDate: Date | null = null;
  calendarWeeks: CalendarDay[][] = [];
  weekDayLabels = ['Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa', 'Su'];

  get currentMonthYear(): string {
    return this.viewDate.toLocaleDateString('en-US', { month: 'long', year: 'numeric' });
  }

  buildCalendar(): void {
    const year = this.viewDate.getFullYear();
    const month = this.viewDate.getMonth();

    // First day of month (0=Sun → convert to Mon-based: 0=Mon)
    const firstDay = new Date(year, month, 1);
    let startDow = firstDay.getDay(); // 0=Sun,1=Mon,...
    // Convert so Monday=0, Sunday=6
    startDow = (startDow === 0) ? 6 : startDow - 1;

    const daysInMonth = new Date(year, month + 1, 0).getDate();
    const daysInPrevMonth = new Date(year, month, 0).getDate();

    const cells: CalendarDay[] = [];

    // Fill leading days from prev month
    for (let i = startDow - 1; i >= 0; i--) {
      const d = daysInPrevMonth - i;
      const fullDate = new Date(year, month - 1, d);
      cells.push({ date: d, fullDate, isCurrentMonth: false, isToday: false, isPast: true });
    }

    // Current month days
    const todayMidnight = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate());
    for (let d = 1; d <= daysInMonth; d++) {
      const fullDate = new Date(year, month, d);
      const isToday = fullDate.getTime() === todayMidnight.getTime();
      const isPast = fullDate < todayMidnight;
      cells.push({ date: d, fullDate, isCurrentMonth: true, isToday, isPast });
    }

    // Fill trailing days from next month
    const remaining = (7 - (cells.length % 7)) % 7;
    for (let d = 1; d <= remaining; d++) {
      const fullDate = new Date(year, month + 1, d);
      cells.push({ date: d, fullDate, isCurrentMonth: false, isToday: false, isPast: false });
    }

    // Chunk into weeks
    this.calendarWeeks = [];
    for (let i = 0; i < cells.length; i += 7) {
      this.calendarWeeks.push(cells.slice(i, i + 7));
    }
  }

  prevMonth(): void {
    this.viewDate = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth() - 1, 1);
    this.buildCalendar();
  }

  nextMonth(): void {
    this.viewDate = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth() + 1, 1);
    this.buildCalendar();
  }

  selectDay(day: CalendarDay): void {
    if (!day.isPast && day.isCurrentMonth) {
      this.selectedDate = day.fullDate;
    }
  }

  isSelected(day: CalendarDay): boolean {
    if (!this.selectedDate) return false;
    return day.fullDate.toDateString() === this.selectedDate.toDateString();
  }

  // ─── Time Slots ────────────────────────────────────────────────
  timeSlots: { label: string; sub: string; available: boolean }[] = [];
  selectedTime = '';

  selectTime(slot: { label: string; sub: string; available: boolean }): void {
    if (slot.available) this.selectedTime = slot.label;
  }

  get formattedDate(): string {
    if (!this.selectedDate) return 'No date selected';
    return this.selectedDate.toLocaleDateString('en-US', {
      weekday: 'long', month: 'short', day: 'numeric', year: 'numeric'
    });
  }

  get formattedTime(): string {
    const slot = this.timeSlots.find(s => s.label === this.selectedTime);
    return slot ? `${slot.label} ${slot.sub}` : '';
  }

  get slotsDateLabel(): string {
    if (!this.selectedDate) return 'Select a date';
    return this.selectedDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
  }

  nextStep(): void {
    if (this.currentStep < 3) this.currentStep++;
  }

  prevStep(): void {
    if (this.currentStep > 1) this.currentStep--;
  }

  confirmAppointment(): void {
    const provider = this.selectedProvider;
    if (!provider) return;

    const slot = this.timeSlots.find(s => s.label === this.selectedTime);
    this.store.addAppointment({
      providerName: provider.name,
      providerSpecialty: provider.specialty,
      providerAvatar: provider.avatarUrl,
      date: this.selectedDate ?? new Date(),
      timeLabel: this.selectedTime,
      timeAmpm: slot?.sub ?? 'AM',
      reason: this.selectedReason,
      notes: this.notes,
      status: 'Upcoming',
      type: 'in-person'
    });

    this.router.navigate(['/app/appointments']);
  }

  cancel(): void {
    this.router.navigate(['/app/dashboard']);
  }
}
