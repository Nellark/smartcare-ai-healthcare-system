import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { ForgotPasswordComponent } from './forgot-password.component';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

describe('ForgotPasswordComponent', () => {
  let component: ForgotPasswordComponent;
  let fixture: ComponentFixture<ForgotPasswordComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let toastService: jasmine.SpyObj<ToastService>;

  beforeEach(async () => {
    authService = jasmine.createSpyObj('AuthService', ['forgotPassword', 'resetPassword']);
    authService.forgotPassword.and.returnValue(of({
      success: true,
      message: 'Reset link sent',
      data: {
        email: 'patient@example.com',
        token: 'reset-token-123',
        resetLink: 'http://localhost:4200/forgot-password?email=patient@example.com&token=reset-token-123'
      }
    }));

    toastService = jasmine.createSpyObj('ToastService', ['success', 'error', 'warning']);

    await TestBed.configureTestingModule({
      imports: [ForgotPasswordComponent],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: authService },
        { provide: ToastService, useValue: toastService },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              queryParamMap: convertToParamMap({})
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ForgotPasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should keep the user on the request form until a secure reset link is opened', () => {
    component.email = 'patient@example.com';
    component.onSubmitRequest();

    expect(component.email).toBe('patient@example.com');
    expect(component.token).toBe('');
    expect(component.stage).toBe(1);
    expect(toastService.success).toHaveBeenCalled();
  });

  it('should load the reset token from the emailed link query parameters', () => {
    const route = {
      snapshot: {
        queryParamMap: convertToParamMap({
          email: 'patient@example.com',
          token: 'reset-token-123'
        })
      }
    } as ActivatedRoute;

    const injector = TestBed.runInInjectionContext(() => {
      return new ForgotPasswordComponent(authService, jasmine.createSpyObj('Router', ['navigate']), route);
    });

    component = injector;
    component.ngOnInit();

    expect(component.email).toBe('patient@example.com');
    expect(component.token).toBe('reset-token-123');
    expect(component.stage).toBe(2);
  });
});
