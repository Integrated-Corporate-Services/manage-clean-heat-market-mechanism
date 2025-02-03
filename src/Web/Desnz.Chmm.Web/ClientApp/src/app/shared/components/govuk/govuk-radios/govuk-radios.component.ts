import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovukRadios } from './govuk-radios.options';

const defaultOptions: GovukRadios = {
  id: '',
  label: '',
  hint: '',
  options: [],
};

@Component({
  selector: 'govuk-radios',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-radios.component.html',
  styleUrls: ['./govuk-radios.component.css'],
})
export class GovukRadiosComponent implements OnChanges {
  @Input({ required: true }) options!: Partial<GovukRadios>;

  state: GovukRadios = defaultOptions;

  ngOnChanges(changes: SimpleChanges): void {
    this.state = { ...defaultOptions, ...changes['options'].currentValue };
  }
}
