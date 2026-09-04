import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeletePatientModalComponent } from './delete-patient-modal.component';

describe('DeletePatientModalComponent', () => {
  let component: DeletePatientModalComponent;
  let fixture: ComponentFixture<DeletePatientModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeletePatientModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeletePatientModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
