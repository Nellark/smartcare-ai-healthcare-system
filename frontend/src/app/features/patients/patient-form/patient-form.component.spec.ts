import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { PatientFormComponent } from './patient-form.component';
import { PatientService } from '../../../core/services/patient.service';

describe('PatientFormComponent', () => {
  let component: PatientFormComponent;
  let fixture: ComponentFixture<PatientFormComponent>;
  let patientServiceSpy: jasmine.SpyObj<PatientService>;

  beforeEach(async () => {
    patientServiceSpy = jasmine.createSpyObj<PatientService>('PatientService', [
      'getById',
      'create',
      'update'
    ]);
    patientServiceSpy.getById.and.returnValue(
      of({
        success: true,
        data: {
          id: '1',
          firstName: 'Jane',
          lastName: 'Doe',
          email: 'jane@example.com',
          dateOfBirth: '1990-01-01',
          phoneNumber: '1234567890',
          address: '123 Main St'
        },
        message: '',
        errors: [],
        timestamp: new Date().toISOString()
      })
    );
    patientServiceSpy.create.and.returnValue(
      of({
        success: true,
        data: {
          id: '1',
          firstName: 'Jane',
          lastName: 'Doe',
          email: 'jane@example.com',
          dateOfBirth: '1990-01-01',
          phoneNumber: '1234567890',
          address: '123 Main St'
        },
        message: '',
        errors: [],
        timestamp: new Date().toISOString()
      })
    );
    patientServiceSpy.update.and.returnValue(
      of({
        success: true,
        data: {
          id: '1',
          firstName: 'Jane',
          lastName: 'Doe',
          email: 'jane@example.com',
          dateOfBirth: '1990-01-01',
          phoneNumber: '1234567890',
          address: '123 Main St'
        },
        message: '',
        errors: [],
        timestamp: new Date().toISOString()
      })
    );

    await TestBed.configureTestingModule({
      imports: [PatientFormComponent],
      providers: [
        provideRouter([]),
        { provide: PatientService, useValue: patientServiceSpy },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: convertToParamMap({})
            }
          }
        }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PatientFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
