import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { PatientListComponent } from './patient-list.component';
import { PatientService } from '../../../core/services/patient.service';

describe('PatientListComponent', () => {
  let component: PatientListComponent;
  let fixture: ComponentFixture<PatientListComponent>;
  let patientServiceSpy: jasmine.SpyObj<PatientService>;

  beforeEach(async () => {
    patientServiceSpy = jasmine.createSpyObj<PatientService>('PatientService', [
      'getAll',
      'search',
      'delete'
    ]);

    patientServiceSpy.getAll.and.returnValue(
      of({
        success: true,
        data: [],
        message: '',
        errors: [],
        timestamp: new Date().toISOString()
      })
    );
    patientServiceSpy.search.and.returnValue(
      of({
        success: true,
        data: [],
        message: '',
        errors: [],
        timestamp: new Date().toISOString()
      })
    );
    patientServiceSpy.delete.and.returnValue(
      of({
        success: true,
        data: null,
        message: '',
        errors: [],
        timestamp: new Date().toISOString()
      })
    );

    await TestBed.configureTestingModule({
      imports: [PatientListComponent],
      providers: [
        { provide: PatientService, useValue: patientServiceSpy }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PatientListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
