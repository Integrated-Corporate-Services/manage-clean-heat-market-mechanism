import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { Component, DestroyRef, Input, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { clearObligationAmendment } from 'src/app/stores/amend-obligation/actions';
import { SchemeYearSummaryConfigurationDto } from 'src/app/stores/organisation-summary/dtos/scheme-year-summary-configuration.dto';
import {
  selectBoilerSalesSummaryData,
  selectObligationCalculationLoading,
  selectObligationSummaryData,
} from 'src/app/stores/organisation-summary/selectors';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  BoilerSalesSubmissionStatus,
  BoilerSalesSummaryDto,
} from 'src/app/stores/organisation-summary/dtos/boiler-sales-summary.dto';
import { ObligationSummaryDto } from 'src/app/stores/organisation-summary/dtos/obligation-summary.dto';
import { Observable } from 'rxjs';
import * as moment from 'moment';
import { numberFormat } from 'src/app/shared/constants';

export interface BoilerSalesTableRow {
  headerName: string;
  total: string;
  aboveThreshold: string;
}

export interface ObligationTableRow {
  headerName: string;
  value: string;
}

@Component({
  selector: 'obligation-calculation',
  templateUrl: './obligation-calculation.component.html',
  styleUrls: ['./obligation-calculation.component.scss'],
  standalone: true,
  imports: [NgFor, NgIf, RouterLink, AsyncPipe],
})
export class ObligationCalculationComponent implements OnInit {
  private destroyRef = inject(DestroyRef);

  @Input({ required: true }) organisationId!: string;
  @Input({ required: true }) isAdmin!: boolean;
  @Input({ required: true })
  schemeYearParameters!: SchemeYearSummaryConfigurationDto;
  @Input({ required: true }) schemeYearId!: string;

  boilerSalesSubmissionStatus: BoilerSalesSubmissionStatus | null = null;

  boilerSalesRows: BoilerSalesTableRow[] = [];
  obligationRows: ObligationTableRow[] = [];
  surrenderDayObligationRows: ObligationTableRow[] = [];

  finalObligations: string | null = null;
  remainingObligations: string | null = null;

  loading$: Observable<boolean>;

  constructor(private store: Store) {
    this.loading$ = this.store.select(selectObligationCalculationLoading);
    this.store
      .select(selectBoilerSalesSummaryData)
      .pipe(takeUntilDestroyed())
      .subscribe((boilerSalesSummary) => {
        if (boilerSalesSummary !== null) {
          this.boilerSalesSubmissionStatus =
            boilerSalesSummary.boilerSalesSubmissionStatus;
          this.setBoilerSalesRows(boilerSalesSummary);
        }
      });
  }

  ngOnInit() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }

    this.store
      .select(selectObligationSummaryData)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((obligationSummary) => {
        if (obligationSummary !== null) {
          this.setObligationRows(obligationSummary);
          this.finalObligations = numberFormat.format(
            obligationSummary.finalObligations
          );

          if (this.schemeYearParameters.isAfterSurrenderDay) {
            this.setSurrenderDayObligationRows(obligationSummary);
            this.remainingObligations = numberFormat.format(
              obligationSummary.remainingObligations
            );
          }
        }
      });
  }

  setBoilerSalesRows(boilerSalesSummary: BoilerSalesSummaryDto) {
    this.boilerSalesRows = [
      {
        headerName: 'Gas boiler sales above threshold (total sales to date)',
        total: numberFormat.format(boilerSalesSummary.gasBoilerSales),
        aboveThreshold: numberFormat.format(
          boilerSalesSummary.gasBoilerSalesAboveThreshold
        ),
      },
      {
        headerName: 'Oil boiler sales above threshold (total sales to date)',
        total: numberFormat.format(boilerSalesSummary.oilBoilerSales),
        aboveThreshold: numberFormat.format(
          boilerSalesSummary.oilBoilerSalesAboveThreshold
        ),
      },
      {
        headerName: 'Total sales above threshold (total of all sales to date)',
        total: numberFormat.format(boilerSalesSummary.sumOfBoilerSales),
        aboveThreshold: numberFormat.format(
          boilerSalesSummary.sumOfBoilerSalesAboveThreshold
        ),
      },
    ];
  }

  setObligationRows(obligationSummary: ObligationSummaryDto) {
    this.obligationRows = [
      {
        headerName: `Low-carbon heat target generated this scheme year`,
        value: numberFormat.format(obligationSummary.generatedObligations),
      },
    ];

    if (
      this.schemeYearParameters.isAfterPreviousSchemeYearSurrenderDate &&
      obligationSummary.obligationsBroughtForward > 0
    ) {
      this.obligationRows.push({
        headerName: `Low-carbon heat target carried forward from previous scheme year`,
        value: numberFormat.format(obligationSummary.obligationsBroughtForward),
      });
    }

    for (const amendment of obligationSummary.obligationAmendments) {
      this.obligationRows.push({
        headerName: `Manually amended by Administrator ${moment(
          amendment.dateOfTransaction
        ).format('DD/MM/YYYY')}`,
        value:
          (amendment.value > 0 ? '+' : '') +
          numberFormat.format(amendment.value),
      });
    }
  }

  setSurrenderDayObligationRows(obligationSummary: ObligationSummaryDto) {
    this.surrenderDayObligationRows = [
      {
        headerName: 'Low-carbon heat target met with credits held',
        value: numberFormat.format(obligationSummary.obligationsPaidOff),
      },
      {
        headerName: 'Low-carbon heat target carried forward to next scheme year',
        value: numberFormat.format(obligationSummary.obligationsCarriedOver),
      },
    ];
  }

  onAmendObligation() {
    this.store.dispatch(clearObligationAmendment());
  }
}
