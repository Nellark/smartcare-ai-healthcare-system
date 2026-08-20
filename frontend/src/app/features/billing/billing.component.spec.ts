import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { BillingComponent } from './billing.component';
import { MockDataService } from '../../core/services/mock-data.service';

describe('BillingComponent', () => {
  let component: BillingComponent;
  let fixture: ComponentFixture<BillingComponent>;
  let mockDataServiceSpy: jasmine.SpyObj<MockDataService>;

  beforeEach(async () => {
    mockDataServiceSpy = jasmine.createSpyObj<MockDataService>('MockDataService', [
      'getBillingData'
    ]);
    mockDataServiceSpy.getBillingData.and.returnValue(
      of({
        totalBalance: '$0',
        dueDate: 'N/A',
        nextPaymentDate: 'N/A',
        autoPayAmount: '$0',
        transactions: [],
        insurance: {
          provider: 'N/A',
          id: 'N/A',
          copay: 'N/A',
          deductible: 'N/A'
        }
      })
    );

    await TestBed.configureTestingModule({
      imports: [BillingComponent],
      providers: [
        { provide: MockDataService, useValue: mockDataServiceSpy }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BillingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
