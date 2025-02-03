import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovukCheckboxes } from './govuk-checkboxes.options';

const defaultOptions: GovukCheckboxes = {
  id: '',
  label: '',
  hint: '',
  options: [],
};

@Component({
  selector: 'govuk-checkboxes',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-checkboxes.component.html',
  styleUrls: ['./govuk-checkboxes.component.css'],
})
export class GovukCheckboxesComponent implements OnChanges {
  @Input({ required: true }) options!: Partial<GovukCheckboxes>;

  state: GovukCheckboxes = defaultOptions;

  ngOnChanges(changes: SimpleChanges): void {
    this.state = { ...defaultOptions, ...changes['options'].currentValue };
  }
}
