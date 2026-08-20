import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { HeaderComponent } from './header.component';
import { AuthService } from '../../core/services/auth.service';
import { ProfileService } from '../../core/services/profile.service';

describe('HeaderComponent', () => {
  let component: HeaderComponent;
  let fixture: ComponentFixture<HeaderComponent>;

  beforeEach(async () => {
    const authServiceMock = {
      currentUser: jasmine.createSpy().and.returnValue('doctor@smartcare.local'),
      currentUserRole: jasmine.createSpy().and.returnValue('Doctor'),
      getRoleLabel: jasmine.createSpy().and.returnValue('Provider'),
      logout: jasmine.createSpy('logout')
    };
    const profileServiceMock = {
      currentProfile: jasmine.createSpy().and.returnValue({
        avatar: '',
        name: 'Dr. Doctor'
      }),
      updateProfile: jasmine.createSpy('updateProfile')
    };

    await TestBed.configureTestingModule({
      imports: [HeaderComponent],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: authServiceMock },
        { provide: ProfileService, useValue: profileServiceMock }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
