import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { LoginMenuComponent } from './login-menu.component';
import { AuthorizeService } from '../authorize.service';
import { createAuthorizeServiceStub } from '../testing/auth-testing.stubs';

const authorizeServiceStub = createAuthorizeServiceStub();

describe('LoginMenuComponent', () => {
  let component: LoginMenuComponent;
  let fixture: ComponentFixture<LoginMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoginMenuComponent],
      providers: [
        provideRouter([]),
        { provide: AuthorizeService, useValue: authorizeServiceStub }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
