import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GovukInputComponent } from './govuk-input.component';

describe('GovukInputComponent', () => {
  let component: GovukInputComponent;
  let fixture: ComponentFixture<GovukInputComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GovukInputComponent],
    });
    fixture = TestBed.createComponent(GovukInputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
