import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';

import { LoginComponent } from './login.component';
import { AuthorizeService } from '../authorize.service';
import { createActivatedRouteStub, createAuthorizeServiceStub } from '../testing/auth-testing.stubs';

const activatedRouteStub = createActivatedRouteStub('login-failed');
const authorizeServiceStub = createAuthorizeServiceStub();

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        provideRouter([]),
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        { provide: AuthorizeService, useValue: authorizeServiceStub }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
