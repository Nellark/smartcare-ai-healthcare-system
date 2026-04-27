import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { DashboardComponent } from './dashboard.component';
import { DashboardService } from '../../core/services/dashboard.service';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;
  let dashboardServiceSpy: jasmine.SpyObj<DashboardService>;

  beforeEach(async () => {
    dashboardServiceSpy = jasmine.createSpyObj<DashboardService>('DashboardService', [
      'getDashboardStats'
    ]);

    dashboardServiceSpy.getDashboardStats.and.returnValue(
      of({
        success: true,
        data: {
          totalPatients: 0,
          totalAppointments: 0,
          totalDoctors: 0,
          recentPatients: [],
          recentAppointments: []
        },
        message: '',
        errors: [],
        timestamp: new Date().toISOString()
      })
    );

    await TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [
        { provide: DashboardService, useValue: dashboardServiceSpy }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
