import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovukErrorSummary } from './govuk-error-summary.options';

const defaultOptions: GovukErrorSummary = {
  title: 'There is a problem',
  errors: [],
};

@Component({
  selector: 'govuk-error-summary',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-error-summary.component.html',
  styleUrls: ['./govuk-error-summary.component.css'],
})
export class GovukErrorSummaryComponent implements OnChanges {
  @Input({ required: true }) options!: Partial<GovukErrorSummary>;

  state: GovukErrorSummary = defaultOptions;

  ngOnChanges(changes: SimpleChanges): void {
    this.state = { ...defaultOptions, ...changes['options'].currentValue };
  }
}
