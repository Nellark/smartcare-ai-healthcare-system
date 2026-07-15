import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

interface CalendarEvent {
  title: string;
  type: 'in-person' | 'telehealth';
}

interface CalendarDay {
  date: number;
  isCurrentMonth: boolean;
  events: CalendarEvent[];
}

@Component({
  selector: 'app-appointments',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './appointments.component.html',
  styleUrls: ['./appointments.component.css']
})
export class AppointmentsComponent {
  daysOfWeek = ['MON', 'TUE', 'WED', 'THU', 'FRI', 'SAT', 'SUN'];
  
  calendarDays: CalendarDay[] = [
    { date: 25, isCurrentMonth: false, events: [] },
    { date: 26, isCurrentMonth: false, events: [{ title: 'J. Doe - In Person', type: 'in-person' }] },
    { date: 27, isCurrentMonth: false, events: [{ title: 'Tele: R. Smith', type: 'telehealth' }] },
    { date: 28, isCurrentMonth: false, events: [{ title: 'A. Brown - In Person', type: 'in-person' }, { title: 'Tele: K. White', type: 'telehealth' }] },
    { date: 29, isCurrentMonth: false, events: [] },
    { date: 30, isCurrentMonth: false, events: [] },
    { date: 31, isCurrentMonth: false, events: [] },
    { date: 1, isCurrentMonth: true, events: [{ title: 'H. Vance - Annual', type: 'in-person' }] },
    { date: 2, isCurrentMonth: true, events: [] },
    { date: 3, isCurrentMonth: true, events: [{ title: 'Tele: L. Moore', type: 'telehealth' }] },
    { date: 4, isCurrentMonth: true, events: [] },
    { date: 5, isCurrentMonth: true, events: [{ title: 'M. Scott - Follow up', type: 'in-person' }] },
    { date: 6, isCurrentMonth: true, events: [] },
    { date: 7, isCurrentMonth: true, events: [] }
  ];

  dailyAgenda = [
    { time: '09:00', ampm: 'AM', name: 'James', desc: 'Post-Op' },
    { time: '10:30', ampm: 'AM', name: 'Sarah C.', desc: 'Lab Result' },
    { time: '01:15', ampm: 'PM', name: 'Robert B.', desc: 'General' }
  ];

  urgentWaitlist = [
    { name: 'Elena Rodriguez', wait: '2.5 hrs' },
    { name: 'Marcus Thorne', wait: '1.2 hrs' }
  ];
}
