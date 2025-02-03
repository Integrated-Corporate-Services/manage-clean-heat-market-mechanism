import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovukInput } from './govuk-input.options';

const defaultOptions: GovukInput = {
  id: '',
  label: '',
  hint: '',
  width: 100,
  units: '',
};

@Component({
  selector: 'govuk-input',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-input.component.html',
  styleUrls: ['./govuk-input.component.css'],
})
export class GovukInputComponent implements OnChanges {
  @Input({ required: true }) options!: Partial<GovukInput>;

  state: GovukInput = defaultOptions;

  ngOnChanges(changes: SimpleChanges): void {
    this.state = { ...defaultOptions, ...changes['options'].currentValue };
  }
}
