import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { Component, DestroyRef, Input, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import * as moment from 'moment';
import { numberFormat } from 'src/app/shared/constants';
import {
  clearCreditAmendment,
  clearCreditTransfer,
} from 'src/app/stores/credit/actions';
import { CreditLedgerSummaryDto } from 'src/app/stores/organisation-summary/dtos/credit-ledger-summary.dto';
import { SchemeYearSummaryConfigurationDto } from 'src/app/stores/organisation-summary/dtos/scheme-year-summary-configuration.dto';
import { selectCreditLedgerSummary } from 'src/app/stores/organisation-summary/selectors';
import { Observable } from 'rxjs';
import { HttpState } from '../../../stores/http-state';
import { selectContactsState } from '../../../stores/contacts/selectors';
import { ContactOrganisationDto } from '../../../stores/onboarding/dtos/contact-organisation-dto';
import { getContactableOrganisations } from '../../../stores/contacts/actions';

export interface CreditTableRow {
  heading: string;
  value: string;
  link: string | null;
}

@Component({
  selector: 'credit-calculation',
  templateUrl: './credit-calculation.component.html',
  styleUrls: ['./credit-calculation.component.scss'],
  standalone: true,
  imports: [NgFor, NgIf, AsyncPipe, RouterLink],
})
export class CreditCalculationComponent implements OnInit {
  private destroyRef = inject(DestroyRef);

  @Input({ required: true }) organisationId!: string;
  @Input({ required: true }) schemeYearId!: string;
  @Input({ required: true }) isAdmin!: boolean;
  @Input({ required: true })
  schemeYearParameters!: SchemeYearSummaryConfigurationDto;

  loading = false;

  creditRows: CreditTableRow[] = [];
  surrenderDayCreditRows: CreditTableRow[] = [];

  organisations$: Observable<HttpState<ContactOrganisationDto[]>>;

  creditBalance: string | null = null;
  creditsExpired: string | null = null;

  constructor(private store: Store, private router: Router) {
    this.organisations$ = this.store.select(selectContactsState);
  }

  ngOnInit() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }

    this.store
      .select(selectCreditLedgerSummary)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((creditLedgerSummary) => {
        this.loading = creditLedgerSummary.loading;

        if (creditLedgerSummary.data !== null) {
          const data = creditLedgerSummary.data;
          this.setCreditRows(data);
          this.creditBalance = numberFormat.format(data.creditBalance);

          if (this.schemeYearParameters.isAfterSurrenderDay) {
            this.setSurrenderDayCreditRows(data);
            this.creditsExpired = numberFormat.format(data.creditsExpired);
          }
        }
      });

    this.store.dispatch(getContactableOrganisations({
      organisationId: this.organisationId,
    }));
  }

  setCreditRows(creditLedgerSummary: CreditLedgerSummaryDto) {
    let heatPumpPlural = this.schemeYearParameters.alternativeRenewableSystemFuelTypeWeightingValue != 1 ? 'credits' : 'credit';
    let hybridHeatPumpPlural = this.schemeYearParameters.alternativeFossilFuelSystemFuelTypeWeightingValue != 1 ? 'credits' : 'credit';
    this.creditRows = [
      {
        link: null,
        heading: `Credits generated from standalone installations (${this.schemeYearParameters.alternativeRenewableSystemFuelTypeWeightingValue} ${heatPumpPlural} per installation)`,
        value: numberFormat.format(
          creditLedgerSummary.creditsGeneratedByHeatPumps
        ),
      },
      {
        link: null,
        heading: `Credits generated from hybrid installations (${this.schemeYearParameters.alternativeFossilFuelSystemFuelTypeWeightingValue} ${hybridHeatPumpPlural} per installation)`,
        value: numberFormat.format(
          creditLedgerSummary.creditsGeneratedByHybridHeatPumps
        ),
      },
      {
        link: '../transfer-history',
        heading: 'Total of credits transferred in and out',
        value: (creditLedgerSummary.creditsTransferred >= 0 ? '+' : '') + numberFormat.format(creditLedgerSummary.creditsTransferred),
      },
    ];

    if (
      this.schemeYearParameters.isAfterPreviousSchemeYearSurrenderDate &&
      creditLedgerSummary.creditsBoughtForward > 0
    ) {
      this.creditRows.push({
        link: null,
        heading: `Credits carried over from previous scheme year`,
        value: numberFormat.format(creditLedgerSummary.creditsBoughtForward),
      });
    }

    for (const amendment of creditLedgerSummary.creditAmendments) {
      this.creditRows.push({
        link: null,
        heading: `Credits amended by Administrator ${moment(
          amendment.dateOfTransaction
        ).format('DD/MM/YYYY')}`,
        value:
          (amendment.credits > 0 ? '+' : '') +
          numberFormat.format(amendment.credits),
      });
    }
  }

  setSurrenderDayCreditRows(creditLedgerSummary: CreditLedgerSummaryDto) {
    this.surrenderDayCreditRows = [
      {
        link: null,
        heading: `Credits redeemed against low carbon heat target`,
        value: numberFormat.format(creditLedgerSummary.creditsRedeemed),
      },
      {
        link: null,
        heading: `Credits carried over to next scheme year`,
        value: numberFormat.format(creditLedgerSummary.creditsCarriedForward),
      },
    ];
  }

  onAmendCredits() {
    this.store.dispatch(clearCreditAmendment());
    this.router.navigate([
      `/scheme-year/${this.schemeYearId}/organisation/${this.organisationId}/amend-credits`,
    ]);
  }

  onTransferCredits() {
    this.store.dispatch(clearCreditTransfer());
    this.router.navigate([
      `/scheme-year/${this.schemeYearId}/organisation/${this.organisationId}/transfer-credits`,
    ]);
  }
}
