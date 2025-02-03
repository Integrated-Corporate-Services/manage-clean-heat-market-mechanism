import { AsyncPipe, DecimalPipe, JsonPipe, NgFor, NgIf } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { CreditCalculationComponent } from '../credit-calculation/credit-calculation.component';
import { ObligationCalculationComponent } from '../obligation-calculation/obligation-calculation.component';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { selectSchemeYearParameters } from 'src/app/stores/organisation-summary/selectors';
import { HttpState } from 'src/app/stores/http-state';
import { SchemeYearSummaryConfigurationDto } from 'src/app/stores/organisation-summary/dtos/scheme-year-summary-configuration.dto';
import { getSchemeYearConfigurationSummary } from 'src/app/stores/organisation-summary/actions';
import { selectIsAdmin } from 'src/app/stores/auth/selectors';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';

@Component({
  selector: 'organisation-summary',
  templateUrl: './organisation-summary.component.html',
  standalone: true,
  imports: [
    DecimalPipe,
    NgFor,
    CreditCalculationComponent,
    ObligationCalculationComponent,
    NgIf,
    AsyncPipe,
    JsonPipe,
  ],
})
export class OrganisationSummaryComponent implements OnInit {
  @Input({ required: true }) organisationId!: string;
  @Input({ required: true }) schemeYearId!: string;

  schemeYearParameters$: Observable<
    HttpState<SchemeYearSummaryConfigurationDto>
  >;
  isAdmin: boolean | null = null;

  constructor(private store: Store, backLinkProvider: BackLinkProvider) {
    backLinkProvider.clear();
    this.schemeYearParameters$ = this.store.select(selectSchemeYearParameters);
    this.store
      .select(selectIsAdmin)
      .pipe(takeUntilDestroyed())
      .subscribe((isAdmin) => {
        this.isAdmin = isAdmin;
      });
  }

  ngOnInit() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }

    this.store.dispatch(
      getSchemeYearConfigurationSummary({
        organisationId: this.organisationId,
        schemeYearId: this.schemeYearId,
      })
    );
  }
}
