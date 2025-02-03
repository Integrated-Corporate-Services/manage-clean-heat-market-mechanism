import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GovukTagComponent } from './govuk-tag.component';

describe('GovukTagComponent', () => {
  let component: GovukTagComponent;
  let fixture: ComponentFixture<GovukTagComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GovukTagComponent],
    });
    fixture = TestBed.createComponent(GovukTagComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
