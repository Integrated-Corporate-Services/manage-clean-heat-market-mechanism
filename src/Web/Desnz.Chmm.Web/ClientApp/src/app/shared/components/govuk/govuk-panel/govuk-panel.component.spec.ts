import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GovukPanelComponent } from './govuk-panel.component';

describe('GovukPanelComponent', () => {
  let component: GovukPanelComponent;
  let fixture: ComponentFixture<GovukPanelComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GovukPanelComponent],
    });
    fixture = TestBed.createComponent(GovukPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
