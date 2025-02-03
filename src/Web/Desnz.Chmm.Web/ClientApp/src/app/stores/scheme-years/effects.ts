import { Actions, ofType, createEffect, concatLatestFrom } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { catchError, map, switchMap } from 'rxjs/operators';
import { NavigationActions, SchemeYearActions } from '..';
import { of } from 'rxjs';
import { SchemeYearService } from '../organisation-summary/services/scheme-year.service';
import { Router } from '@angular/router';
import { SessionStorageService } from 'src/app/shared/services/session-storage.service';
import { previousSchemeYearSessionKey, schemeYearSessionKey } from 'src/app/shared/constants';
import { Store } from '@ngrx/store';
import { selectIsAdmin, selectWhoAmI } from '../auth/selectors';
import { isAdmin } from 'src/app/shared/auth-utils';
import { getPreviousSchemeYear } from './actions';

@Injectable()
export class SchemeYearsEffects {
  constructor(
    private actions$: Actions,
    private schemeYearService: SchemeYearService,
    private router: Router,
    private sessionStorageService: SessionStorageService,
    private store: Store
  ) { }

  getSchemeYears$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(SchemeYearActions.getSchemeYears),
      switchMap((_) => {
        return this.schemeYearService.getAllSchemeYears().pipe(
          map((schemeYears) => {
            return SchemeYearActions.onGetSchemeYears({
              schemeYears,
            });
          }),
          catchError((error) =>
            of(
              SchemeYearActions.onGetSchemeYearsError({
                message: error.error?.detail,
              })
            )
          )
        );
      })
    );
  });

  getPreviousSchemeYear$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(SchemeYearActions.getPreviousSchemeYear),
      switchMap((action) => {
        return this.schemeYearService.getSchemeYear(action.schemeYearId).pipe(
          map((previousSchemeYear) => {
            this.sessionStorageService.setObject(previousSchemeYearSessionKey, previousSchemeYear);
            return of();
          }),
          catchError((error) =>
            of(
              SchemeYearActions.onGetSchemeYearsError({
                message: error.error?.detail,
              })
            )
          )
        );
      })
    );
  }, { dispatch: false });

  goToSchemeYear$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(SchemeYearActions.goToSchemeYear),
        concatLatestFrom((_) => this.store.select(selectWhoAmI)),
        map(([action, whoAmI]) => {
          if (action.schemeYear.previousSchemeYearId) {
            this.store.dispatch(getPreviousSchemeYear({
              schemeYearId: action.schemeYear.previousSchemeYearId
            }));
          }
          this.sessionStorageService.setObject(
            schemeYearSessionKey,
            action.schemeYear
          );
          isAdmin(whoAmI)
            ? this.router.navigate(
              [`organisation/${action.organisationId}/edit/check-answers`],
              {
                queryParams: {
                  fetchData: true,
                  mode: 'edit',
                  redirectToSummary: true,
                  schemeYearId: action.schemeYear.id,
                },
              }
            )
            : this.router.navigate([
              `scheme-year/${action.schemeYear.id}/organisation/${action.organisationId}/summary`,
            ]);
        })
      );
    },
    { dispatch: false }
  );

  toggleSchemeYearSelector$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(SchemeYearActions.goToSchemeYear),
      concatLatestFrom((_) => this.store.select(selectIsAdmin)),
      switchMap(([_, isAdmin]) => {
        return isAdmin
          ? [NavigationActions.toggleSchemeYearSelector({ show: true })]
          : [
            NavigationActions.toggleSchemeYearSelector({ show: true }),
            NavigationActions.toggleServiceNav({ show: true }),
          ];
      })
    );
  });
}
