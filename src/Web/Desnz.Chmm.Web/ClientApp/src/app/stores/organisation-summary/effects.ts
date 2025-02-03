import { Actions, createEffect, ofType } from '@ngrx/effects';
import { SchemeYearService } from './services/scheme-year.service';
import { OrganisationSummaryActions } from '..';
import { catchError, map, of, switchMap } from 'rxjs';
import { Injectable } from '@angular/core';
import { BoilerSalesService } from '../boiler-sales/services/boiler-sales-service';
import { ObligationService } from '../amend-obligation/services/obligation.service';
import { CreditService } from '../credit/services/credit.service';

@Injectable()
export class OrganisationSummaryEffects {
  constructor(
    private actions$: Actions,
    private configurationService: SchemeYearService,
    private boilerSalesService: BoilerSalesService,
    private obligationService: ObligationService,
    private creditService: CreditService
  ) {}

  getSchemeYearParameters$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OrganisationSummaryActions.getSchemeYearConfigurationSummary),
      switchMap((action) => {
        return this.configurationService
          .getSchemeYearSummaryConfiguration(action.schemeYearId)
          .pipe(
            map((schemeYearParameters) => {
              return OrganisationSummaryActions.onGetSchemeYearConfigurationSummary(
                {
                  organisationId: action.organisationId,
                  schemeYearId: action.schemeYearId,
                  schemeYearParameters: schemeYearParameters,
                }
              );
            }),
            catchError((error) =>
              of(
                OrganisationSummaryActions.onGetSchemeYearConfigurationSummaryError(
                  {
                    errorMessage: error.error?.detail,
                  }
                )
              )
            )
          );
      })
    );
  });

  onGetSchemeYearParametersGetBoilerSalesSummary$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OrganisationSummaryActions.onGetSchemeYearConfigurationSummary),
      map((action) => {
        return OrganisationSummaryActions.getBoilerSalesSummary({
          organisationId: action.organisationId,
          schemeYearId: action.schemeYearId,
        });
      })
    );
  });

  onGetSchemeYearParametersGetObligationSummary$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OrganisationSummaryActions.onGetSchemeYearConfigurationSummary),
      map((action) => {
        return OrganisationSummaryActions.getObligationSummary({
          organisationId: action.organisationId,
          schemeYearId: action.schemeYearId,
        });
      })
    );
  });

  onGetSchemeYearParametersGetCreditLedgerSummary$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OrganisationSummaryActions.onGetSchemeYearConfigurationSummary),
      map((action) => {
        return OrganisationSummaryActions.getCreditLedgerSummary({
          organisationId: action.organisationId,
          schemeYearId: action.schemeYearId,
        });
      })
    );
  });

  getBoilerSalesSummary$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OrganisationSummaryActions.getBoilerSalesSummary),
      switchMap((action) => {
        return this.boilerSalesService
          .getBoilerSalesSummary(action.organisationId, action.schemeYearId)
          .pipe(
            map((boilerSalesSummary) => {
              return OrganisationSummaryActions.onGetBoilerSalesSummary({
                boilerSalesSummary,
              });
            }),
            catchError((error) =>
              of(
                OrganisationSummaryActions.onGetBoilerSalesSummaryError({
                  errorMessage: error.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  getObligationSummary$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OrganisationSummaryActions.getObligationSummary),
      switchMap((action) => {
        return this.obligationService
          .getObligationSummary(action.organisationId, action.schemeYearId)
          .pipe(
            map((obligationSummary) => {
              return OrganisationSummaryActions.onGetObligationSummary({
                obligationSummary,
              });
            }),
            catchError((error) =>
              of(
                OrganisationSummaryActions.onGetObligationSummaryError({
                  errorMessage: error.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  getCreditLedgerSummary$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OrganisationSummaryActions.getCreditLedgerSummary),
      switchMap((action) => {
        return this.creditService
          .getCreditLedgerSummary(action.organisationId, action.schemeYearId)
          .pipe(
            map((creditLedgerSummary) => {
              return OrganisationSummaryActions.onGetCreditLedgerSummary({
                creditLedgerSummary,
              });
            }),
            catchError((error) =>
              of(
                OrganisationSummaryActions.onGetCreditLedgerSummaryError({
                  errorMessage: error.error?.detail,
                })
              )
            )
          );
      })
    );
  });
}
