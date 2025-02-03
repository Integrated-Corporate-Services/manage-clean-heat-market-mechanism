import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GovukRadiosComponent } from './govuk-radios.component';

describe('GovukRadiosComponent', () => {
  let component: GovukRadiosComponent;
  let fixture: ComponentFixture<GovukRadiosComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GovukRadiosComponent],
    });
    fixture = TestBed.createComponent(GovukRadiosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
