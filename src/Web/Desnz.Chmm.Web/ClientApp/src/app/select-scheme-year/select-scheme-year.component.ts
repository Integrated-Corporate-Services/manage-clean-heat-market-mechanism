import { Component, Input, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import { SchemeYearDto } from '../stores/organisation-summary/dtos/scheme-year.dto';
import { selectSchemeYearsState } from '../stores/scheme-years/selectors';
import { HttpState } from '../stores/http-state';
import { AsyncPipe, JsonPipe, NgFor, NgIf } from '@angular/common';
import { getSchemeYears, goToSchemeYear } from '../stores/scheme-years/actions';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { selectWhoAmI } from '../stores/auth/selectors';
import { RouterLink } from '@angular/router';
import { Observable } from 'rxjs';

@Component({
  selector: 'select-scheme-year',
  templateUrl: './select-scheme-year.component.html',
  styleUrls: ['./select-scheme-year.component.scss'],
  standalone: true,
  imports: [FormsModule, NgIf, AsyncPipe, NgFor, RouterLink],
})
export class SelectSchemeYearComponent implements OnInit {
  @Input() organisationId: string | null = null;
  schemeYears$: Observable<HttpState<SchemeYearDto[]>>;
  schemeYearId: string | null = null;
  error: string | null = null;

  constructor(private store: Store) {
    this.schemeYears$ = this.store.select(selectSchemeYearsState);

    this.store
      .select(selectWhoAmI)
      .pipe(takeUntilDestroyed())
      .subscribe((user) => {
        if (user?.organisationId) {
          this.organisationId = user.organisationId;
        }
      });
  }

  ngOnInit() {
    this.store.dispatch(getSchemeYears());
  }

  onGoToSchemeYear(schemeYears: SchemeYearDto[]) {
    this.error = null;
    if (this.schemeYearId === null) {
      this.error = 'Select scheme year';
    }

    if (this.organisationId === null) {
      throw Error('organisationId cannot be null');
    }

    const schemeYear = schemeYears.find((s) => s.id === this.schemeYearId);
    if (!schemeYear) {
      throw Error('schemeYear not found');
    }

    if (!this.error) {
      this.store.dispatch(
        goToSchemeYear({
          schemeYear: schemeYear,
          organisationId: this.organisationId,
        })
      );
    }
  }
}
