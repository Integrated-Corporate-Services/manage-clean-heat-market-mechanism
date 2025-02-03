import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GovukNotificationBannerComponent } from './govuk-notification-banner.component';

describe('GovukNotificationBannerComponent', () => {
  let component: GovukNotificationBannerComponent;
  let fixture: ComponentFixture<GovukNotificationBannerComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GovukNotificationBannerComponent],
    });
    fixture = TestBed.createComponent(GovukNotificationBannerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
