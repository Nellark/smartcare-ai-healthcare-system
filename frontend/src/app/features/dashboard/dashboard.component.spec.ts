import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { DashboardComponent } from './dashboard.component';
import { DashboardService } from '../../core/services/dashboard.service';
import { PatientService } from '../../core/services/patient.service';
import { AuthService } from '../../core/services/auth.service';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;
  let dashboardServiceSpy: jasmine.SpyObj<DashboardService>;
  let patientServiceSpy: jasmine.SpyObj<PatientService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    dashboardServiceSpy = jasmine.createSpyObj<DashboardService>('DashboardService', [
      'getDashboardStats'
    ]);
    patientServiceSpy = jasmine.createSpyObj<PatientService>('PatientService', [
      'getAll',
      'create',
      'delete'
    ]);
    authServiceSpy = jasmine.createSpyObj<AuthService>('AuthService', [
      'currentUser',
      'currentUserRole',
      'getRoleLabel'
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
    patientServiceSpy.getAll.and.returnValue(
      of({
        success: true,
        data: [],
        message: '',
        errors: [],
        timestamp: new Date().toISOString()
      })
    );
    authServiceSpy.currentUser.and.returnValue('doctor@smartcare.local');
    authServiceSpy.currentUserRole.and.returnValue('Doctor');
    authServiceSpy.getRoleLabel.and.callFake((role) => role === 'Doctor' ? 'Provider' : 'User');

    await TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [
        { provide: DashboardService, useValue: dashboardServiceSpy },
        { provide: PatientService, useValue: patientServiceSpy },
        { provide: AuthService, useValue: authServiceSpy }
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
