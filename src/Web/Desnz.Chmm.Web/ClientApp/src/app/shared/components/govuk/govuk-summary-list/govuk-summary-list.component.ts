import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  GovukSummaryList,
  GovukSummaryListRow,
} from './govuk-summary-list.options';
import { Router } from '@angular/router';

const defaultOptions: GovukSummaryList = {
  rows: [],
};

@Component({
  selector: 'govuk-summary-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-summary-list.component.html',
  styleUrls: ['./govuk-summary-list.component.css'],
})
export class GovukSummaryListComponent implements OnChanges {
  @Input({ required: true }) options!: Partial<GovukSummaryList>;

  state: GovukSummaryList = defaultOptions;

  constructor(private router: Router) {}

  ngOnChanges(changes: SimpleChanges): void {
    this.state = { ...defaultOptions, ...changes['options'].currentValue };
  }

  public change(row: Partial<GovukSummaryListRow>) {
    this.router.navigateByUrl(row.link ?? '/');
  }
}
