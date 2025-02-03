import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovukTextarea } from './govuk-textarea.options';

const defaultOptions: GovukTextarea = {
  id: '',
  label: '',
  hint: '',
  rows: 5,
};

@Component({
  selector: 'govuk-textarea',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-textarea.component.html',
  styleUrls: ['./govuk-textarea.component.css'],
})
export class GovukTextareaComponent implements OnChanges {
  @Input({ required: true }) options!: Partial<GovukTextarea>;

  state: GovukTextarea = defaultOptions;

  ngOnChanges(changes: SimpleChanges): void {
    this.state = { ...defaultOptions, ...changes['options'].currentValue };
  }
}
