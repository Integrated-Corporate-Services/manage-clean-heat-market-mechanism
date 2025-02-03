import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovukDetails } from './govuk-details.options';

const defaultOptions: GovukDetails = {
  summary: '',
  details: '',
};
@Component({
  selector: 'govuk-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-details.component.html',
  styleUrls: ['./govuk-details.component.css'],
})
export class GovukDetailsComponent implements OnChanges {
  @Input({ required: true }) options!: Partial<GovukDetails>;

  state: GovukDetails = defaultOptions;

  ngOnChanges(changes: SimpleChanges): void {
    this.state = { ...defaultOptions, ...changes['options'].currentValue };
  }
}
