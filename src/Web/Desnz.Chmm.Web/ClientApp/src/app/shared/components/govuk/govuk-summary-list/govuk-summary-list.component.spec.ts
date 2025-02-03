import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GovukSummaryListComponent } from './govuk-summary-list.component';

describe('GovukSummaryListComponent', () => {
  let component: GovukSummaryListComponent;
  let fixture: ComponentFixture<GovukSummaryListComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GovukSummaryListComponent],
    });
    fixture = TestBed.createComponent(GovukSummaryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
