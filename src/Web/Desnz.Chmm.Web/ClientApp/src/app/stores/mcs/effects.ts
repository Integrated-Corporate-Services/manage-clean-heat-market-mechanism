import { Actions, ofType, createEffect } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { catchError, map, switchMap } from 'rxjs/operators';
import { McsActions } from '..';
import { McsService } from './services/mcs.service';
import { of } from 'rxjs';

@Injectable()
export class McsEffects {
  constructor(private actions$: Actions, private mcsService: McsService) {}

  getMcsDownloads$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(McsActions.getMcsDownloads),
      switchMap((_) => {
        return this.mcsService.getMcsDownloads(_.schemeYearId).pipe(
          map((downloads) => {
            return McsActions.onGetMcsDownloads({
              downloads,
            });
          }),
          catchError((error) =>
            of(McsActions.onError({ message: error.error?.detail }))
          )
        );
      })
    );
  });
}
