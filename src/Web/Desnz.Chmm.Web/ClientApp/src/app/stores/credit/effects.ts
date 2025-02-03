import { Actions, ofType, createEffect, concatLatestFrom } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { catchError, exhaustMap, filter, map, switchMap } from 'rxjs/operators';
import { CreditActions } from '..';
import { CreditService } from './services/credit.service';
import { of } from 'rxjs';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import {
  selectCreditTransfer,
  selectEditCreditAmendment,
  selectOrganisationId,
} from './selectors';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable()
export class CreditEffects {
  constructor(
    private actions$: Actions,
    private creditService: CreditService,
    private router: Router,
    private store: Store
  ) { }

  getCreditBalance$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(CreditActions.getCreditBalance),
      switchMap((action) => {
        return this.creditService
          .getCreditBalance(action.organisationId, action.schemeYearId)
          .pipe(
            map((creditBalanceDto) => {
              return CreditActions.onGetCreditBalance({
                creditBalance: creditBalanceDto.creditBalance,
              });
            }),
            catchError((error) =>
              of(CreditActions.onError({ message: error.error?.detail }))
            )
          );
      })
    );
  });

  getCreditTransferHistory$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(CreditActions.getCreditTransferHistory),
      switchMap((action) => {
        return this.creditService
          .getCreditTransferHistory(action.organisationId, action.schemeYearId)
          .pipe(
            map((transferHistory) => {
              return CreditActions.onGetCreditTransferHistory({
                transferHistory: transferHistory
              });
            }),
            catchError((error) =>
              of(CreditActions.onError({ message: error.error?.detail }))
            )
          );
      })
    );
  });

  storeCreditTransfer$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(CreditActions.storeCreditTransfer),
        map((action) => {
          this.router.navigate([
            `/scheme-year/${action.schemeYearId}/organisation/${action.organisationId}/transfer-credits/check-answers`,
          ]);
        })
      );
    },
    { dispatch: false }
  );

  submitCreditTransfer$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(CreditActions.submitCreditTransfer),
      concatLatestFrom((_) => this.store.select(selectCreditTransfer)),
      filter((creditTransfer) => creditTransfer != null),
      exhaustMap(([action, creditTransfer]) => {
        return this.creditService
          .submitCreditTransfer(
            action.organisationId,
            creditTransfer!,
            action.schemeYearId
          )
          .pipe(
            map((_) => {
              return CreditActions.onSubmitCreditTransfer({
                organisationId: action.organisationId,
                schemeYearId: action.schemeYearId,
              });
            }),
            catchError((errorResponse: HttpErrorResponse) =>
              of(
                CreditActions.onError({
                  message: errorResponse.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  onSubmitCreditTransfer$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(CreditActions.onSubmitCreditTransfer),
        map((action) => {
          this.router.navigate([
            `/scheme-year/${action.schemeYearId}/organisation/${action.organisationId}/transfer-credits/confirmation`,
          ]);
        })
      );
    },
    { dispatch: false }
  );

  storeCreditAmendment$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(CreditActions.storeCreditAmendment),
        concatLatestFrom((_) => [this.store.select(selectOrganisationId)]),
        map(([action, organisationId]) => {
          this.router.navigate([
            `/scheme-year/${action.schemeYearId}/organisation/${organisationId}/amend-credits/check-answers`,
          ]);
        })
      );
    },
    { dispatch: false }
  );

  submitCreditAmendment$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(CreditActions.submitCreditAmendment),
      concatLatestFrom((_) => [
        this.store.select(selectEditCreditAmendment),
        this.store.select(selectOrganisationId),
      ]),
      exhaustMap(([action, creditAmendment, organisationId]) => {
        if (creditAmendment === null || organisationId === null) {
          throw TypeError('CreditAmendment & OrganisationId cannot be null');
        }
        return this.creditService
          .submitCreditAmendment(
            organisationId,
            creditAmendment,
            action.schemeYearId
          )
          .pipe(
            map((_) => {
              return CreditActions.onSubmitCreditAmendment({
                schemeYearId: action.schemeYearId,
              });
            }),
            catchError((error) =>
              of(
                CreditActions.onSubmitCreditAmendmentError({
                  message: error.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  onSubmitCreditAmendment$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(CreditActions.onSubmitCreditAmendment),
        concatLatestFrom((_) => this.store.select(selectOrganisationId)),
        map(([action, organisationId]) => {
          this.router.navigate([
            `/scheme-year/${action.schemeYearId}/organisation/${organisationId}/amend-credits/confirmation`,
          ]);
        })
      );
    },
    { dispatch: false }
  );
}
