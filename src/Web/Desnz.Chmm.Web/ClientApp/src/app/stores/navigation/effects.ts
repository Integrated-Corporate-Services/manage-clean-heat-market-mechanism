import { Actions, ofType, createEffect } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { navigationOrganisationSessionKey } from 'src/app/shared/constants';
import { SessionStorageService } from 'src/app/shared/services/session-storage.service';
import { NavigationActions } from '..';

@Injectable()
export class NavigationEffects {
  constructor(
    private actions$: Actions,
    private sessionStorageService: SessionStorageService
  ) {}

  navigateNext$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(NavigationActions.toggleSecondaryNav),
        map((action) => {
          if (!action.show) {
            this.sessionStorageService.remove(navigationOrganisationSessionKey);
          }
        })
      );
    },
    { dispatch: false }
  );
}
