import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { MainLayoutComponent } from './main-layout.component';
import { AuthService } from '../../core/services/auth.service';
import { ProfileService } from '../../core/services/profile.service';

describe('MainLayoutComponent', () => {
  let component: MainLayoutComponent;
  let fixture: ComponentFixture<MainLayoutComponent>;

  beforeEach(async () => {
    const authServiceMock = {
      currentUser: jasmine.createSpy().and.returnValue('admin@smartcare.local'),
      currentUserRole: jasmine.createSpy().and.returnValue('Admin'),
      getRoleLabel: jasmine.createSpy().and.returnValue('Admin'),
      logout: jasmine.createSpy('logout')
    };
    const profileServiceMock = {
      currentProfile: jasmine.createSpy().and.returnValue({
        avatar: '',
        name: 'Admin User'
      }),
      updateProfile: jasmine.createSpy('updateProfile')
    };

    await TestBed.configureTestingModule({
      imports: [MainLayoutComponent],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: authServiceMock },
        { provide: ProfileService, useValue: profileServiceMock }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MainLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
