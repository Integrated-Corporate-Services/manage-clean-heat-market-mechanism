import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GovukSelectComponent } from './govuk-select.component';

describe('GovukSelectComponent', () => {
  let component: GovukSelectComponent;
  let fixture: ComponentFixture<GovukSelectComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GovukSelectComponent],
    });
    fixture = TestBed.createComponent(GovukSelectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
