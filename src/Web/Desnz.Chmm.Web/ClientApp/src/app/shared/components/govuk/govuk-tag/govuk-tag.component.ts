import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovukTag } from './govuk-tag.options';

const defaultOptions: GovukTag = {
  text: '',
  colour: 'none',
};

@Component({
  selector: 'govuk-tag',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-tag.component.html',
  styleUrls: ['./govuk-tag.component.css'],
})
export class GovukTagComponent implements OnChanges {
  @Input({ required: true }) options!: Partial<GovukTag>;

  state: GovukTag = defaultOptions;

  ngOnChanges(changes: SimpleChanges): void {
    this.state = { ...defaultOptions, ...changes['options'].currentValue };
  }
}
