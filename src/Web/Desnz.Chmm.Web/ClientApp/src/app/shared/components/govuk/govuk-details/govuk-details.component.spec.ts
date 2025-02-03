import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GovukDetailsComponent } from './govuk-details.component';

describe('GovukDetailsComponent', () => {
  let component: GovukDetailsComponent;
  let fixture: ComponentFixture<GovukDetailsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GovukDetailsComponent],
    });
    fixture = TestBed.createComponent(GovukDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
