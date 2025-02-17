import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GovukCheckboxesComponent } from './govuk-checkboxes.component';

describe('GovukCheckboxesComponent', () => {
  let component: GovukCheckboxesComponent;
  let fixture: ComponentFixture<GovukCheckboxesComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GovukCheckboxesComponent],
    });
    fixture = TestBed.createComponent(GovukCheckboxesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
