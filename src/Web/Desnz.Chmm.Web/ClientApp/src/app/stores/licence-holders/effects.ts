import { Actions, ofType, createEffect, concatLatestFrom } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { catchError, exhaustMap, map, switchMap } from 'rxjs/operators';
import * as LicenceHolderActions from './actions';
import { LicenceHolderService } from './services/licence-holder.service';
import { of } from 'rxjs';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import {
  selectCurrentSchemeYear,
  selectEditLinkedLicenceHolderDto,
  selectLinkLicenceHolderDto,
  selectSelectedLicenceHolderLink,
} from './selectors';
import { SchemeYearService } from '../organisation-summary/services/scheme-year.service';

@Injectable()
export class LicenceHolderEffects {
  constructor(
    private actions$: Actions,
    private router: Router,
    private licenceHolderService: LicenceHolderService,
    private store: Store,
    private schemeYearService: SchemeYearService
  ) {}

  getAllLicenceHolders$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(LicenceHolderActions.getAllLicenceHolders),
      switchMap((_) => {
        return this.licenceHolderService.getAllLicenceHolders().pipe(
          map((licenceHolders) => {
            return LicenceHolderActions.onGetAllLicenceHolders({
              licenceHolders,
            });
          }),
          catchError((error) =>
            of(
              LicenceHolderActions.onErrorGetAllLicenceHolders({
                message: error.error?.detail,
              })
            )
          )
        );
      })
    );
  });

  getLinkedLicenceHolders$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(LicenceHolderActions.getLinkedLicenceHolders),
      concatLatestFrom((_) => this.store.select(selectCurrentSchemeYear)),
      switchMap(([action, schemeYear]) => {
        return this.licenceHolderService
          .getLinkedLicenceHolders(action.organisationId)
          .pipe(
            map((licenceHolderLinks) => {
              return LicenceHolderActions.onGetLinkedLicenceHolders({
                licenceHolderLinks: licenceHolderLinks,
                schemeYearStartDate: schemeYear.data!.startDate,
              });
            }),
            catchError((error) =>
              of(
                LicenceHolderActions.onErrorGetLinkedLicenceHolders({
                  message: error.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  getUnlinkedLicenceHolders$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(LicenceHolderActions.getUnlinkedLicenceHolders),
      switchMap((_) => {
        return this.licenceHolderService.getUnlinkedLicenceHolders().pipe(
          map((licenceHolders) => {
            return LicenceHolderActions.onGetUnlinkedLicenceHolders({
              licenceHolders,
            });
          }),
          catchError((error) =>
            of(
              LicenceHolderActions.onErrorGetUnlinkedLicenceHolders({
                message: error.error?.detail,
              })
            )
          )
        );
      })
    );
  });

  linkLicenceHolder$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(LicenceHolderActions.linkLicenceHolder),
        concatLatestFrom((_) => this.store.select(selectLinkLicenceHolderDto)),
        switchMap(([action, licenceHolderDto]) => {
          return this.licenceHolderService
            .linkLicenceHolder(
              action.licenceHolderId,
              action.organisationId,
              licenceHolderDto
            )
            .pipe(
              map((_) => {
                return this.router.navigate([
                  `/organisation/${action.organisationId}/licence-holders/${action.licenceHolderId}/linked`,
                ]);
              }),
              catchError((error) =>
                of(
                  LicenceHolderActions.onErrorLinkLicenceHolder({
                    message: error.error?.detail,
                  })
                )
              )
            );
        })
      );
    },
    { dispatch: false }
  );

  storeLinkLicenceHolderFormValue$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(LicenceHolderActions.storeLinkLicenceHolderFormValue),
        map((action) => {
          this.router.navigate([
            `/organisation/${action.organisationId}/licence-holders/${action.linkLicenceHolderFormValue.selectedLicenceHolderId}/confirm-link`,
          ]);
        })
      );
    },
    { dispatch: false }
  );

  getFirstSchemeYear$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(LicenceHolderActions.getFirstSchemeYear),
      switchMap((_) => {
        return this.schemeYearService.getFirstSchemeYear().pipe(
          map((schemeYear) => {
            return LicenceHolderActions.onGetFirstSchemeYear({
              schemeYear: schemeYear,
            });
          }),
          catchError((error) =>
            of(
              LicenceHolderActions.onGetFirstSchemeYearError({
                message: error.error?.detail,
              })
            )
          )
        );
      })
    );
  });

  getCurrentSchemeYear$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(LicenceHolderActions.getCurrentSchemeYear),
      switchMap((action) => {
        return this.schemeYearService.getCurrentSchemeYear().pipe(
          map((schemeYear) => {
            return LicenceHolderActions.onGetCurrentSchemeYear({
              organisationId: action.organisationId,
              schemeYear: schemeYear,
            });
          }),
          catchError((error) =>
            of(
              LicenceHolderActions.onGetCurrentSchemeYearError({
                message: error.error?.detail,
              })
            )
          )
        );
      })
    );
  });

  onGetCurrentSchemeYear$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(LicenceHolderActions.onGetCurrentSchemeYear),
      map((action) => {
        return LicenceHolderActions.getLinkedLicenceHolders({
          organisationId: action.organisationId,
        });
      })
    );
  });

  clearLinkLicenceHolderFormValue$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(LicenceHolderActions.clearLinkLicenceHolderFormValue),
        map((action) => {
          this.router.navigate([
            `/organisation/${action.organisationId}/licence-holders/link`,
          ]);
        })
      );
    },
    { dispatch: false }
  );

  selectLincenceHolder$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(LicenceHolderActions.selectLinkedLincenceHolder),
        map((action) => {
          this.router.navigate([
            `/organisation/${action.organisationId}/licence-holders/${action.licenceHolderLink.licenceHolderId}/edit`,
          ]);
        })
      );
    },
    { dispatch: false }
  );

  storeEditLinkedLicenceHolderFormValue$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(LicenceHolderActions.storeEditLinkedLicenceHolderFormValue),
        map((action) => {
          this.router.navigate([
            `/organisation/${action.organisationId}/licence-holders/${action.licenceHolderId}/confirm-edit`,
          ]);
        })
      );
    },
    { dispatch: false }
  );

  editLinkedLicenceHolder$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(LicenceHolderActions.editLinkedLicenceHolder),
      concatLatestFrom((_) =>
        this.store.select(selectEditLinkedLicenceHolderDto)
      ),
      exhaustMap(([action, editLinkedLicenceHolderDto]) => {
        return this.licenceHolderService
          .endLink(
            action.licenceHolderId,
            action.organisationId,
            editLinkedLicenceHolderDto
          )
          .pipe(
            map((_) => {
              return LicenceHolderActions.onEditLinkedLicenceHolder({
                licenceHolderId: action.licenceHolderId,
              });
            }),
            catchError((error) =>
              of(
                LicenceHolderActions.onEditLinkedLicenceHolderError({
                  message: error.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  onEditLinkedLicenceHolder$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(LicenceHolderActions.onEditLinkedLicenceHolder),
        concatLatestFrom((_) =>
          this.store.select(selectSelectedLicenceHolderLink)
        ),
        map(([action, licenceHolderLink]) => {
          this.router.navigate([
            `/organisation/${licenceHolderLink?.organisationId}/licence-holders/${action.licenceHolderId}/edited`,
          ]);
        })
      );
    },
    { dispatch: false }
  );
}
