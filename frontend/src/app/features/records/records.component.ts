import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MockDataService } from '../../core/services/mock-data.service';

interface RecordItem {
  type: string;
  status: string;
  statusClass: string;
  title: string;
  provider: string;
  subtext: string;
  date: string;
  dateLabel: string;
}

@Component({
  selector: 'app-records',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './records.component.html',
  styleUrls: ['./records.component.css']
})
export class RecordsComponent implements OnInit {
  private mockDataService = inject(MockDataService);

  records: RecordItem[] = [];
  currentPage = 1;
  totalPages = 1;
  totalRecords = 0;

  ngOnInit(): void {
    this.mockDataService.getRecordsData().subscribe({
      next: (data) => {
        this.records = data;
        this.totalRecords = data.length;
        this.totalPages = Math.ceil(data.length / 10) || 1;
      }
    });
  }
}
