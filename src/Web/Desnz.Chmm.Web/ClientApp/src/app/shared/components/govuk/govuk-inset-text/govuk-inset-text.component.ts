import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovukInsetText } from './govuk-inset-text.options';

const defaultOptions: GovukInsetText = {
  text: '',
};

@Component({
  selector: 'govuk-inset-text',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-inset-text.component.html',
  styleUrls: ['./govuk-inset-text.component.css'],
})
export class GovukInsetTextComponent implements OnChanges {
  @Input({ required: true }) options!: Partial<GovukInsetText>;

  state: GovukInsetText = defaultOptions;

  ngOnChanges(changes: SimpleChanges): void {
    this.state = { ...defaultOptions, ...changes['options'].currentValue };
  }
}
