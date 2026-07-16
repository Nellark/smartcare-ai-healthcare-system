import { Component, inject, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ConsultationStoreService, BookedAppointment } from '../../core/services/consultation-store.service';

export interface CalendarCell {
  date: number;
  fullDate: Date;
  isCurrentMonth: boolean;
  isToday: boolean;
  events: BookedAppointment[];
}

export interface WeekDay {
  label: string;       // "Mon", "Tue" …
  shortDate: string;   // "16"
  fullDate: Date;
  isToday: boolean;
  events: BookedAppointment[];
}

@Component({
  selector: 'app-appointments',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './appointments.component.html',
  styleUrls: ['./appointments.component.css']
})
export class AppointmentsComponent {
  private store = inject(ConsultationStoreService);

  today = new Date();

  // viewDate anchors: Month → 1st of month; Week/Day → any day in range
  viewDate = signal(new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate()));
  activeView = signal<'Month' | 'Week' | 'Day'>('Month');
  views: Array<'Month' | 'Week' | 'Day'> = ['Month', 'Week', 'Day'];

  daysOfWeek = ['MON', 'TUE', 'WED', 'THU', 'FRI', 'SAT', 'SUN'];

  // Time slots for Week/Day views (07:00 – 19:00)
  timeSlots = Array.from({ length: 13 }, (_, i) => {
    const h = i + 7;
    const ampm = h < 12 ? 'AM' : h === 12 ? 'PM' : 'PM';
    const display = h <= 12 ? h : h - 12;
    return { hour: h, label: `${String(display).padStart(2, '0')}:00 ${ampm}` };
  });

  // ─── Header label ───────────────────────────────────────────────
  headerLabel = computed(() => {
    const v = this.activeView();
    const d = this.viewDate();
    if (v === 'Month') {
      return d.toLocaleDateString('en-US', { month: 'long', year: 'numeric' });
    }
    if (v === 'Week') {
      const { start, end } = this.weekRange(d);
      const s = start.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
      const e = end.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
      return `${s} – ${e}`;
    }
    return d.toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric', year: 'numeric' });
  });

  // ─── Navigation ─────────────────────────────────────────────────
  setView(v: 'Month' | 'Week' | 'Day'): void {
    this.activeView.set(v);
  }

  prev(): void {
    const v = this.activeView();
    this.viewDate.update(d => {
      const nd = new Date(d);
      if (v === 'Month') nd.setMonth(nd.getMonth() - 1);
      else if (v === 'Week') nd.setDate(nd.getDate() - 7);
      else nd.setDate(nd.getDate() - 1);
      return nd;
    });
  }

  next(): void {
    const v = this.activeView();
    this.viewDate.update(d => {
      const nd = new Date(d);
      if (v === 'Month') nd.setMonth(nd.getMonth() + 1);
      else if (v === 'Week') nd.setDate(nd.getDate() + 7);
      else nd.setDate(nd.getDate() + 1);
      return nd;
    });
  }

  goToday(): void {
    this.viewDate.set(new Date(this.today));
  }

  // ─── Month view ──────────────────────────────────────────────────
  calendarWeeks = computed(() => {
    const d = this.viewDate();
    const year = d.getFullYear();
    const month = d.getMonth();
    const appts = this.store.appointments();

    let startDow = new Date(year, month, 1).getDay();
    startDow = startDow === 0 ? 6 : startDow - 1;

    const daysInMonth = new Date(year, month + 1, 0).getDate();
    const daysInPrev  = new Date(year, month, 0).getDate();
    const todayMid    = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate());

    const cells: CalendarCell[] = [];

    for (let i = startDow - 1; i >= 0; i--) {
      const day = daysInPrev - i;
      cells.push({ date: day, fullDate: new Date(year, month - 1, day), isCurrentMonth: false, isToday: false, events: [] });
    }
    for (let day = 1; day <= daysInMonth; day++) {
      const fullDate = new Date(year, month, day);
      const isToday  = fullDate.getTime() === todayMid.getTime();
      const events   = appts.filter(a => {
        const ad = new Date(a.date);
        return ad.getFullYear() === year && ad.getMonth() === month && ad.getDate() === day && a.status !== 'Cancelled';
      });
      cells.push({ date: day, fullDate, isCurrentMonth: true, isToday, events });
    }
    const trailing = (7 - (cells.length % 7)) % 7;
    for (let day = 1; day <= trailing; day++) {
      cells.push({ date: day, fullDate: new Date(year, month + 1, day), isCurrentMonth: false, isToday: false, events: [] });
    }

    const weeks: CalendarCell[][] = [];
    for (let i = 0; i < cells.length; i += 7) weeks.push(cells.slice(i, i + 7));
    return weeks;
  });

  // ─── Week view ───────────────────────────────────────────────────
  private weekRange(d: Date): { start: Date; end: Date } {
    const dow = d.getDay();  // 0=Sun
    const diff = (dow === 0 ? -6 : 1 - dow); // shift to Monday
    const start = new Date(d);
    start.setDate(d.getDate() + diff);
    start.setHours(0, 0, 0, 0);
    const end = new Date(start);
    end.setDate(start.getDate() + 6);
    return { start, end };
  }

  weekDays = computed((): WeekDay[] => {
    const { start } = this.weekRange(this.viewDate());
    const appts = this.store.appointments();
    const todayStr = this.today.toDateString();
    const dayNames = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

    return Array.from({ length: 7 }, (_, i) => {
      const fd = new Date(start);
      fd.setDate(start.getDate() + i);
      const events = appts.filter(a => new Date(a.date).toDateString() === fd.toDateString() && a.status !== 'Cancelled');
      return {
        label: dayNames[fd.getDay()],
        shortDate: String(fd.getDate()),
        fullDate: fd,
        isToday: fd.toDateString() === todayStr,
        events
      };
    });
  });

  eventsForSlot(day: WeekDay, hour: number): BookedAppointment[] {
    return day.events.filter(e => {
      const h = parseInt(e.timeLabel.split(':')[0], 10);
      const isPM = e.timeAmpm === 'PM' && h !== 12;
      const eventHour = isPM ? h + 12 : h;
      return eventHour === hour;
    });
  }

  // ─── Day view ────────────────────────────────────────────────────
  dayEvents = computed(() => {
    const d = this.viewDate();
    const dStr = d.toDateString();
    return this.store.appointments().filter(a => new Date(a.date).toDateString() === dStr && a.status !== 'Cancelled');
  });

  eventsForDaySlot(hour: number): BookedAppointment[] {
    return this.dayEvents().filter(e => {
      const h = parseInt(e.timeLabel.split(':')[0], 10);
      const isPM = e.timeAmpm === 'PM' && h !== 12;
      return (isPM ? h + 12 : h) === hour;
    });
  }

  isDayToday = computed(() => this.viewDate().toDateString() === this.today.toDateString());

  // ─── Sidebar ─────────────────────────────────────────────────────
  todayAgenda = computed(() =>
    this.store.appointments()
      .filter(a => new Date(a.date).toDateString() === this.today.toDateString() && a.status !== 'Cancelled')
      .sort((a, b) => a.timeLabel.localeCompare(b.timeLabel))
  );

  urgentWaitlist = [
    { name: 'Elena Rodriguez', wait: '2.5 hrs' },
    { name: 'Marcus Thorne',   wait: '1.2 hrs' }
  ];

  occupancyRate = 88;
}
