import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovukTable } from './govuk-table.options';

const defaultOptions: GovukTable = {
  caption: '',
  header: [],
  body: [],
};

@Component({
  selector: 'govuk-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-table.component.html',
  styleUrls: ['./govuk-table.component.css'],
})
export class GovukTableComponent implements OnChanges {
  @Input({ required: true }) options!: Partial<GovukTable>;

  state: GovukTable = defaultOptions;

  ngOnChanges(changes: SimpleChanges): void {
    this.state = { ...defaultOptions, ...changes['options'].currentValue };
  }
}
