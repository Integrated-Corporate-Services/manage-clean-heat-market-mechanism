import { NgIf } from '@angular/common';
import { Component, DestroyRef, Input, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';
import { selectSelectedSchemeYear } from 'src/app/stores/scheme-years/selectors';

@Component({
  selector: 'boiler-sales-quarterly-confirmation',
  templateUrl: './boiler-sales-quarterly-confirmation.component.html',
  standalone: true,
  imports: [NgIf],
})
export class BoilerSalesQuarterlyConfirmationComponent implements OnInit {
  private destroyRef = inject(DestroyRef);

  @Input() organisationId?: string;
  @Input() schemeYearId?: string;
  @Input() schemeYearQuarterId?: string;
  @Input() mode!: 'submit' | 'edit';

  year: string | null = null;
  quarter?: string | null = null;

  constructor(private store: Store) {}

  ngOnInit() {
    this.store
      .select(selectSelectedSchemeYear)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((schemeYear) => {
        if (schemeYear) {
          this.year = schemeYear.year;
          this.quarter = schemeYear.quarters.find(
            (q) => q.id === this.schemeYearQuarterId
          )?.name;
        }
      });
  }
}
