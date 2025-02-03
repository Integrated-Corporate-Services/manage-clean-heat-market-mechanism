import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovukWarningText } from './govuk-warning-text.options';

const defaultOptions: GovukWarningText = {
  text: '',
};

@Component({
  selector: 'govuk-warning-text',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-warning-text.component.html',
  styleUrls: ['./govuk-warning-text.component.css'],
})
export class GovukWarningTextComponent implements OnChanges {
  @Input({ required: true }) options!: Partial<GovukWarningText>;

  state: GovukWarningText = defaultOptions;

  ngOnChanges(changes: SimpleChanges): void {
    this.state = { ...defaultOptions, ...changes['options'].currentValue };
  }
}
