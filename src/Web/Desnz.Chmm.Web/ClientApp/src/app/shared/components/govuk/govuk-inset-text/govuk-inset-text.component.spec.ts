import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GovukInsetTextComponent } from './govuk-inset-text.component';

describe('GovukInsetTextComponent', () => {
  let component: GovukInsetTextComponent;
  let fixture: ComponentFixture<GovukInsetTextComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GovukInsetTextComponent],
    });
    fixture = TestBed.createComponent(GovukInsetTextComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
