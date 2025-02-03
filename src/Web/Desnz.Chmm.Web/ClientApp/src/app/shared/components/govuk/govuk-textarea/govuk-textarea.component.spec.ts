import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GovukTextareaComponent } from './govuk-textarea.component';

describe('GovukTextareaComponent', () => {
  let component: GovukTextareaComponent;
  let fixture: ComponentFixture<GovukTextareaComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GovukTextareaComponent],
    });
    fixture = TestBed.createComponent(GovukTextareaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
