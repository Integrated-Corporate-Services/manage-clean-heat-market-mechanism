import { Actions, ofType, createEffect } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { catchError, exhaustMap, map } from 'rxjs/operators';
import * as HeatPumpInstallationsActions from './actions';
import { of } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { CreditService } from '../credit/services/credit.service';

@Injectable()
export class HeatPumpInstallationsEffects {
  constructor(
    private actions$: Actions,
    private creditService: CreditService
  ) {}

  getHeatPumpInstallations$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(HeatPumpInstallationsActions.getHeatPumpInstallations),
      exhaustMap((action) => {
        return this.creditService
          .getHeatPumpInstallations(action.organisationId, action.schemeYearId)
          .pipe(
            map((heatPumpInstallations) => {
              return HeatPumpInstallationsActions.onGetHeatPumpInstallations({
                heatPumpInstallations
              });
            }),
            catchError((error: HttpErrorResponse) =>
              of(
                HeatPumpInstallationsActions.onGetHeatPumpInstallationsError({
                  error: error.error?.detail,
                })
              )
            )
          );
      })
    );
  });
}
