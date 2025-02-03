import { Actions, ofType, createEffect, concatLatestFrom } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { catchError, exhaustMap, map, switchMap } from 'rxjs/operators';
import { ObligationActions } from '..';
import { of } from 'rxjs';
import { ObligationService } from './services/obligation.service';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import {
  selectEditObligationAmendment,
  selectOrganisationId,
} from './selectors';

@Injectable()
export class ObligationEffects {
  constructor(
    private actions$: Actions,
    private obligationService: ObligationService,
    private router: Router,
    private store: Store
  ) {}

  storeObligationAmendment$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(ObligationActions.storeObligationAmendment),
        concatLatestFrom((_) => [this.store.select(selectOrganisationId)]),
        map(([action, organisationId]) => {
          this.router.navigate([
            `/scheme-year/${action.schemeYearId}/organisation/${organisationId}/amend-obligation/check-answers`,
          ]);
        })
      );
    },
    { dispatch: false }
  );

  submitObligationAmendment$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(ObligationActions.submitObligationAmendment),
      concatLatestFrom((_) => [
        this.store.select(selectEditObligationAmendment),
        this.store.select(selectOrganisationId),
      ]),
      exhaustMap(([action, obligationAmendment, organisationId]) => {
        if (obligationAmendment === null || organisationId === null) {
          throw TypeError(
            'ObligationAmendment & OrganisationId cannot be null'
          );
        }
        return this.obligationService
          .submitObligationAmendment(
            organisationId,
            action.schemeYearId,
            obligationAmendment
          )
          .pipe(
            map((_) => {
              return ObligationActions.onSubmitObligationAmendment({
                schemeYearId: action.schemeYearId,
              });
            }),
            catchError((error) =>
              of(
                ObligationActions.onSubmitObligationAmendmentError({
                  message: error.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  onSubmitObligationAmendment$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(ObligationActions.onSubmitObligationAmendment),
        concatLatestFrom((_) => this.store.select(selectOrganisationId)),
        map(([action, organisationId]) => {
          this.router.navigate([
            `/scheme-year/${action.schemeYearId}/organisation/${organisationId}/amend-obligation/confirmation`,
          ]);
        })
      );
    },
    { dispatch: false }
  );
}
