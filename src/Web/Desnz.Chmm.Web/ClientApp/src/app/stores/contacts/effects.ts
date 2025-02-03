import { Actions, ofType, createEffect } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { catchError, map, switchMap } from 'rxjs/operators';
import { ContactsActions } from '..';
import { of } from 'rxjs';
import { OrganisationService } from '../onboarding/services.ts/organisation-service';

@Injectable()
export class ContactsEffects {
  constructor(
    private actions$: Actions,
    private organisationService: OrganisationService
  ) {}

  getContactableOrganisations$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(ContactsActions.getContactableOrganisations),
      switchMap((action) => {
        return this.organisationService
          .getContactableOrganisations(action.organisationId)
          .pipe(
            map((organisations) => {
              return ContactsActions.onGetContactableOrganisations({
                organisations,
              });
            }),
            catchError((error) =>
              of(
                ContactsActions.onGetContactableOrganisationsError({
                  message: error.error?.detail,
                })
              )
            )
          );
      })
    );
  });
}
