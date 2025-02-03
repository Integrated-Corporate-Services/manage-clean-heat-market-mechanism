import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovukSelect } from './govuk-select.options';

const defaultOptions: GovukSelect = {
  id: '',
  label: '',
  hint: '',
  options: [],
};

@Component({
  selector: 'govuk-select',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-select.component.html',
  styleUrls: ['./govuk-select.component.css'],
})
export class GovukSelectComponent implements OnChanges {
  @Input({ required: true }) options!: Partial<GovukSelect>;

  state: GovukSelect = defaultOptions;

  ngOnChanges(changes: SimpleChanges): void {
    this.state = { ...defaultOptions, ...changes['options'].currentValue };
  }
}
