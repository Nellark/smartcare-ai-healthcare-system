import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MockDataService } from '../../core/services/mock-data.service';

interface UpcomingVisit {
  type: string;
  badge: string;
  title: string;
  description: string;
  physician?: string;
  room?: string;
  date?: string;
  time?: string;
  imageUrl?: string;
  hasPreparation?: boolean;
}

interface HistoryVisit {
  service: string;
  clinician: string;
  date: string;
  status: 'COMPLETED' | 'ARCHIVED';
  docText: string;
  iconType: string;
}

@Component({
  selector: 'app-appointments',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './appointments.component.html',
  styleUrls: ['./appointments.component.css']
})
export class AppointmentsComponent implements OnInit {
  private mockDataService = inject(MockDataService);

  upcomingVisits: UpcomingVisit[] = [];
  historyVisits: HistoryVisit[] = [];

  ngOnInit(): void {
    this.mockDataService.getAppointmentsData().subscribe({
      next: (data) => {
        this.upcomingVisits = data.upcomingVisits;
        this.historyVisits = data.historyVisits;
      }
    });
  }
}
