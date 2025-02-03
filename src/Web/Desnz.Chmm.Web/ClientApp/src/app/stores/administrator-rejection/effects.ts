import { Actions, ofType, createEffect } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { catchError, exhaustMap, map } from 'rxjs/operators';
import * as AdministratorRejectionActions from './actions';
import { of } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { OrganisationService } from '../onboarding/services.ts/organisation-service';

@Injectable()
export class AdministratorRejectionEffects {
  constructor(
    private actions$: Actions,
    private organisationService: OrganisationService
  ) {}

  getApprovalComments$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(AdministratorRejectionActions.getRejectionComments),
      exhaustMap((action) => {
        return this.organisationService
          .getRejectionComments(action.organisationId)
          .pipe(
            map((organisationRejectionCommentsDto) => {
              return AdministratorRejectionActions.onGetRejectionComments({
                organisationRejectionCommentsDto,
              });
            }),
            catchError((error: HttpErrorResponse) =>
              of(
                AdministratorRejectionActions.onGetRejectionCommentsError({
                  error: error.error?.detail,
                })
              )
            )
          );
      })
    );
  });
}
