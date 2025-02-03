import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovukPanel } from './govuk-panel.options';

const defaultOptions: GovukPanel = {
  title: '',
  message: '',
  reference: '',
};

@Component({
  selector: 'govuk-panel',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-panel.component.html',
  styleUrls: ['./govuk-panel.component.css'],
})
export class GovukPanelComponent implements OnChanges {
  @Input({ required: true }) options!: Partial<GovukPanel>;

  state: GovukPanel = defaultOptions;

  ngOnChanges(changes: SimpleChanges): void {
    this.state = { ...defaultOptions, ...changes['options'].currentValue };
  }
}
