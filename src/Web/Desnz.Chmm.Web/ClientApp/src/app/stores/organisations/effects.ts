import { Actions, ofType, createEffect } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { catchError, map, switchMap } from 'rxjs/operators';
import { OrganisationsActions } from '..';
import { of } from 'rxjs';
import { OrganisationService } from '../onboarding/services.ts/organisation-service';

@Injectable()
export class AvailableForTransferEffects {
  constructor(
    private actions$: Actions,
    private organisationService: OrganisationService
  ) {}

  getOrganisationsAvailableForTransfer$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OrganisationsActions.getOrganisationsAvailableForTransfer),
      switchMap((action) => {
        return this.organisationService
          .getAvailableForTransfer(action.organisationId)
          .pipe(
            map((organisations) => {
              return OrganisationsActions.onGetOrganisationsAvailableForTransfer(
                {
                  organisations,
                }
              );
            }),
            catchError((error) =>
              of(
                OrganisationsActions.onGetOrganisationsAvailableForTransferError(
                  {
                    message: error.error?.detail,
                  }
                )
              )
            )
          );
      })
    );
  });
}
