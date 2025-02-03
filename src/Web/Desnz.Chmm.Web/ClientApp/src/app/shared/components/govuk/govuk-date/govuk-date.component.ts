import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovukDate } from './govuk-date.options';

const defaultOptions: GovukDate = {
  id: '',
  label: '',
  hint: '',
};

@Component({
  selector: 'govuk-date',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-date.component.html',
  styleUrls: ['./govuk-date.component.css'],
})
export class GovukDateComponent implements OnChanges {
  @Input({ required: true }) options!: Partial<GovukDate>;

  state: GovukDate = defaultOptions;

  ngOnChanges(changes: SimpleChanges): void {
    this.state = { ...defaultOptions, ...changes['options'].currentValue };
  }
}
