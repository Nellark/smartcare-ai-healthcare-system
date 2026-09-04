import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MockDataService } from '../../core/services/mock-data.service';

export interface Transaction {
  date: string;
  year: string;
  title: string;
  invoiceId: string;
  status: 'PROCESSING' | 'PAID BY INSURANCE' | 'OVERDUE' | 'COMPLETED';
  amount: string;
}

export interface InsuranceInfo {
  provider: string;
  id: string;
  copay: string;
  deductible: string;
}

@Component({
  selector: 'app-billing',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './billing.component.html',
  styleUrls: ['./billing.component.css']
})
export class BillingComponent implements OnInit {
  private mockDataService = inject(MockDataService);

  totalBalance = '';
  dueDate = '';
  nextPaymentDate = '';
  autoPayAmount = '';
  
  transactions: Transaction[] = [];
  
  insurance: InsuranceInfo = {
    provider: '',
    id: '',
    copay: '',
    deductible: ''
  };

  ngOnInit(): void {
    this.mockDataService.getBillingData().subscribe({
      next: (data) => {
        this.totalBalance = data.totalBalance;
        this.dueDate = data.dueDate;
        this.nextPaymentDate = data.nextPaymentDate;
        this.autoPayAmount = data.autoPayAmount;
        this.transactions = data.transactions;
        this.insurance = data.insurance;
      }
    });
  }
}
