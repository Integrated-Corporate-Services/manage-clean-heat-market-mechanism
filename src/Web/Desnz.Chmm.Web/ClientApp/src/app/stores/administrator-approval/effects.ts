import { Actions, ofType, createEffect } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { catchError, exhaustMap, map } from 'rxjs/operators';
import * as AdministratorApprovalActions from './actions';
import { of } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { OrganisationService } from '../onboarding/services.ts/organisation-service';

@Injectable()
export class AdministratorApprovalEffects {
  constructor(
    private actions$: Actions,
    private organisationService: OrganisationService
  ) {}

  getApprovalComments$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(AdministratorApprovalActions.getApprovalComments),
      exhaustMap((action) => {
        return this.organisationService
          .getApprovalComments(action.organisationId)
          .pipe(
            map((organisationApprovalCommentsDto) => {
              return AdministratorApprovalActions.onGetApprovalComments({
                organisationApprovalCommentsDto,
              });
            }),
            catchError((error: HttpErrorResponse) =>
              of(
                AdministratorApprovalActions.onGetApprovalCommentsError({
                  error: error.error?.detail,
                })
              )
            )
          );
      })
    );
  });
}
