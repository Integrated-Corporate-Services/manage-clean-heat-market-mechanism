import { Actions, ofType, createEffect, concatLatestFrom } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { catchError, map, switchMap } from 'rxjs/operators';
import { SchemeYearConfigurationActions } from '..';
import { forkJoin, of } from 'rxjs';
import { SchemeYearConfigurationService } from './services/scheme-year-configuration.service';
import { SessionStorageService } from '../../shared/services/session-storage.service';
import { SchemeYearDto } from './dtos/scheme-year.dto';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { selectUpdateSchemeYearConfigurationCommand } from './selectors';
import { SchemeYearConfigurationDto } from './dtos/scheme-year-configuration.dto';
import { UpdateSchemeYearConfigurationCommand } from './commands/update-scheme-year-configuration.command';

@Injectable()
export class SchemeYearConfigurationEffects {
  constructor(
    private actions$: Actions,
    private store: Store,
    private schemeYearService: SchemeYearConfigurationService,
    private sessionStorageService: SessionStorageService,
    private router: Router
  ) { }

  getSchemeYears$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(SchemeYearConfigurationActions.getSchemeYears),
      switchMap((_) => {
        const dateTimeOverride = this.sessionStorageService.getObject<string>('dateTimeOverride');
        return this.schemeYearService.getSchemeYears().pipe(
          map((schemeYears) => {
            let orderedSchemeYears = schemeYears
              .sort((a, b) => Number(b.year) - Number(a.year))
              .map(schemeYear => this.mapSchemeYear(dateTimeOverride, schemeYear));
            return SchemeYearConfigurationActions.onGetSchemeYears({
              schemeYears: orderedSchemeYears
            });
          }),
          catchError((error) =>
            of(
              SchemeYearConfigurationActions.onGetSchemeYearsError({
                message: error.error?.detail
              })
            )
          )
        );
      })
    );
  });

  getSchemeYear$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(SchemeYearConfigurationActions.getSchemeYear),
      switchMap((action) => {
        const dateTimeOverride = this.sessionStorageService.getObject<string>('dateTimeOverride');
        let getSchemeYear = this.schemeYearService.getSchemeYear(action.schemeYearId);
        let getSchemeYearConfiguration = this.schemeYearService.getSchemeYearConfiguration(action.schemeYearId);
        return forkJoin({ getSchemeYear, getSchemeYearConfiguration }).pipe(
          map(responses => {
            return SchemeYearConfigurationActions.onGetSchemeYear({
              schemeYear: this.mapSchemeYear(dateTimeOverride, responses.getSchemeYear),
              schemeYearConfiguration: responses.getSchemeYearConfiguration
            });
          }),
          catchError((error) =>
            of(
              SchemeYearConfigurationActions.onGetSchemeYearError({
                message: error.error?.detail
              })
            )
          )
        );
      })
    );
  });

  storeSchemeYearConfiguration$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(SchemeYearConfigurationActions.storeSchemeYearConfiguration),
      map((action) => {
        this.router.navigateByUrl(`scheme-year/${action.schemeYearConfiguration.schemeYearId}/configuration/check-answers`);
      })
    );
  }, { dispatch: false });

  saveSchemeYearConfiguration$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(SchemeYearConfigurationActions.saveSchemeYearConfiguration),
      concatLatestFrom((_) => this.store.select(selectUpdateSchemeYearConfigurationCommand)),
      switchMap(([action, command]) => {
        return this.schemeYearService.updateSchemeYearConfiguration(command!).pipe(
          map((_) => {
            this.router.navigateByUrl(`scheme-year/${command!.schemeYearId}/configuration/confirmation`);
          })
        );
      })
    );
  }, { dispatch: false });

  private mapSchemeYear(dateTimeOverride: string | null, schemeYear: SchemeYearDto): SchemeYearDto {
    let currentDate = dateTimeOverride ? Date.parse(dateTimeOverride) : Date.now();
    let startDate = Date.parse(schemeYear.startDate);
    return { ...schemeYear, isReadOnly: startDate <= currentDate };
  }
}
