import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ViewManageChmmComponent } from './view-manage-chmm.component';
import { AuthenticationService } from '../stores/auth/services/authentication.service';
import {
  provideHttpClient,
  withInterceptorsFromDi,
} from '@angular/common/http';
import { SessionStorageService } from '../shared/services/session-storage.service';

describe('ViewManageChmmComponent', () => {
  let component: ViewManageChmmComponent;
  let fixture: ComponentFixture<ViewManageChmmComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ViewManageChmmComponent, RouterTestingModule],
      providers: [
        AuthenticationService,
        SessionStorageService,
        provideHttpClient(withInterceptorsFromDi()),
      ],
    });
    fixture = TestBed.createComponent(ViewManageChmmComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
