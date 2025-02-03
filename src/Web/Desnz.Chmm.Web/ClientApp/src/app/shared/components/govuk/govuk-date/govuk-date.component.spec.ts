import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GovukDateComponent } from './govuk-date.component';

describe('GovukDateComponent', () => {
  let component: GovukDateComponent;
  let fixture: ComponentFixture<GovukDateComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GovukDateComponent],
    });
    fixture = TestBed.createComponent(GovukDateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
