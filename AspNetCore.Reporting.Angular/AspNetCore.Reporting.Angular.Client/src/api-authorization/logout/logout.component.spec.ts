import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { LogoutComponent } from './logout.component';
import { AuthorizeService } from '../authorize.service';
import { createActivatedRouteStub, createAuthorizeServiceStub } from '../testing/auth-testing.stubs';

const activatedRouteStub = createActivatedRouteStub('logged-out');
const authorizeServiceStub = createAuthorizeServiceStub();

describe('LogoutComponent', () => {
  let component: LogoutComponent;
  let fixture: ComponentFixture<LogoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LogoutComponent],
      providers: [
        provideRouter([]),
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        { provide: AuthorizeService, useValue: authorizeServiceStub }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LogoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
