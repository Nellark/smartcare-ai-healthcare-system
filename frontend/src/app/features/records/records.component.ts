import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

interface RecordMock {
  patientName: string;
  patientId: string;
  initials: string;
  type: string;
  subType?: string;
  typeColor: string;
  referenceCode: string;
  dateCreated: string;
  status: 'Finalized' | 'Pending Review' | 'High Priority';
  provider: string;
}

@Component({
  selector: 'app-records',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './records.component.html',
  styleUrls: ['./records.component.css']
})
export class RecordsComponent implements OnInit {
  activeTab = 'All Records';
  tabs = ['All Records', 'Lab Results', 'Imaging', 'Clinical Notes'];

  records: RecordMock[] = [
    {
      patientName: 'Eleanor Johnson',
      patientId: '#SMT-99201',
      initials: 'EJ',
      type: 'IMAGING',
      subType: '(MRI)',
      typeColor: 'blue',
      referenceCode: 'LUMBAR-SPINE-V2',
      dateCreated: 'Oct 24, 2023',
      status: 'Finalized',
      provider: 'Dr. Patel'
    },
    {
      patientName: 'Marcus Holloway',
      patientId: '#SMT-44312',
      initials: 'MH',
      type: 'LAB',
      subType: 'RESULT',
      typeColor: 'green',
      referenceCode: 'CBC-W-DIFF',
      dateCreated: 'Oct 23, 2023',
      status: 'Pending Review',
      provider: 'LabCorp Systems'
    },
    {
      patientName: 'Sarah Chen',
      patientId: '#SMT-00125',
      initials: 'SC',
      type: 'NOTE',
      typeColor: 'purple',
      referenceCode: 'FOLLOW-UP-POST-OP',
      dateCreated: 'Oct 22, 2023',
      status: 'Finalized',
      provider: 'Dr. Smith'
    },
    {
      patientName: 'Robert Bradley',
      patientId: '#SMT-33811',
      initials: 'RB',
      type: 'IMAGING',
      subType: '(X-RAY)',
      typeColor: 'blue',
      referenceCode: 'CHEST-AP-LAT',
      dateCreated: 'Oct 22, 2023',
      status: 'High Priority',
      provider: 'Radiology Dept.'
    },
    {
      patientName: 'Katherine Long',
      patientId: '#SMT-11200',
      initials: 'KL',
      type: 'LAB',
      subType: 'RESULT',
      typeColor: 'green',
      referenceCode: 'LIPID-PANEL',
      dateCreated: 'Oct 21, 2023',
      status: 'Finalized',
      provider: 'Dr. Patel'
    }
  ];

  ngOnInit(): void {}
}
