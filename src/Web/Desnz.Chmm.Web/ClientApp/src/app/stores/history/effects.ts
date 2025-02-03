import { Actions, ofType, createEffect } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { catchError, map, switchMap } from 'rxjs/operators';
import { HistoryActions } from '..';
import { of } from 'rxjs';
import { SystemAuditService } from './services/SystemAuditService';

@Injectable()
export class HistoryEffects {
  constructor(
    private actions$: Actions,
    private systemAuditService: SystemAuditService
  ) {}

  getAuditItems$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(HistoryActions.getAuditItems),
      switchMap((action) => {
        return this.systemAuditService
          .getAuditItems(action.organisationId)
          .pipe(
            map((auditItems) => {
              return HistoryActions.onGetAuditItems({
                auditItems,
              });
            }),
            catchError((error) =>
              of(
                HistoryActions.onGetAuditItemsError({
                  message: error.error?.detail,
                })
              )
            )
          );
      })
    );
  });
}
