import { Actions, ofType, createEffect, concatLatestFrom } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { catchError, exhaustMap, map, switchMap } from 'rxjs/operators';
import * as OnboardingActions from './actions';
import { NavigationExtras, Params, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import {
  selectAccountRejectionFiles,
  selectAccountApprovalFiles,
  selectForApproveOrganisationDto,
  selectForEditOrganisationDto,
  selectForRejectOrganisationDto,
  selectForSubmitOnboardingDetails,
  selectIsNonSchemePartipant,
  selectOrgId,
  selectOrganisationId,
} from './selectors';
import { OrganisationService } from './services.ts/organisation-service';
import { forkJoin, of } from 'rxjs';
import { OnboardingFormMode } from 'src/app/manufacturer/onboarding/models/mode';
import { HttpErrorResponse } from '@angular/common/http';
import { navigationOrganisationSessionKey } from 'src/app/shared/constants';
import { Organisation } from 'src/app/navigation/models/organisation';
import { SessionStorageService } from 'src/app/shared/services/session-storage.service';
import { NavigationActions } from '..';
import { selectSelectedSchemeYear } from '../scheme-years/selectors';

@Injectable()
export class OnboardingEffects {
  constructor(
    private actions$: Actions,
    private router: Router,
    private store: Store,
    private organisationService: OrganisationService,
    private sessionStorageService: SessionStorageService
  ) {}

  storeJourneyForm$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.storeIsOnBehalfOfGroup),
      map((action) => {
        return action.mode === 'approve' || action.mode === 'edit'
          ? OnboardingActions.editAccount({ area: 'IsOnBehalfOfGroup' })
          : OnboardingActions.navigateNext({
              mode: action.mode,
              next: '/organisation/register/responsible-undertaking',
            });
      })
    );
  });

  storeResponsibleUndertaking$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.storeResponsibleUndertaking),
      map((action) => {
        return action.mode === 'approve' || action.mode === 'edit'
          ? OnboardingActions.editAccount({ area: 'ResponsibleUndertaking' })
          : OnboardingActions.navigateNext({
              mode: action.mode,
              next: '/organisation/register/registered-office-address',
            });
      })
    );
  });

  storeAddress$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.storeAddress),
      map((action) => {
        return action.mode === 'approve' || action.mode === 'edit'
          ? OnboardingActions.editAccount({ area: 'Address' })
          : OnboardingActions.navigateNext({
              mode: action.mode,
              next: '/organisation/register/legal-correspondence-address',
            });
      })
    );
  });

  storeIsNonSchemeParticipant$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.storeIsNonSchemeParticipant),
      map((action) => {
        return action.mode === 'approve' || action.mode === 'edit'
          ? OnboardingActions.editAccount({ area: 'IsNonSchemeParticipant' })
          : OnboardingActions.navigateNext({
              mode: action.mode,
              next: '/organisation/register/fossil-fuel-boilers',
            });
      })
    );
  });

  storeLegalCorrespondenceAddress$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.storeLegalCorrespondenceAddress),
      map((action) => {
        return action.mode === 'approve' || action.mode === 'edit'
          ? OnboardingActions.editAccount({
              area: 'LegalCorrespondenceAddress',
            })
          : OnboardingActions.navigateNext({
              mode: action.mode,
              next: '/organisation/register/fossil-fuel-boilers',
            });
      })
    );
  });

  storeIsFossilFuelBoilerSeller$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.storeIsFossilFuelBoilerSeller),
      map((action) => {
        return action.mode === 'approve' || action.mode === 'edit'
          ? OnboardingActions.editAccount({ area: 'IsFossilFuelBoilerSeller' })
          : OnboardingActions.navigateNext({
              mode: action.mode,
              next: '/organisation/register/heat-pump-brands',
            });
      })
    );
  });

  storeHeatPumpBrands$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.storeHeatPumpBrands),
      map((action) => {
        return action.mode === 'approve' || action.mode === 'edit'
          ? OnboardingActions.editAccount({ area: 'HeatPumpBrands' })
          : OnboardingActions.navigateNext({
              mode: action.mode,
              next: '/organisation/register/user-details',
            });
      })
    );
  });

  storeUserDetails$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.storeUserDetails),
      map((action) => {
        return action.mode === 'approve' || action.mode === 'edit'
          ? OnboardingActions.editAccount({ area: 'UserDetails' })
          : OnboardingActions.navigateNext({
              mode: action.mode,
              next: '/organisation/register/responsible-officer-form',
            });
      })
    );
  });

  storeResponsibleOfficer$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.storeResponsibleOfficer),
      map((action) => {
        return action.mode === 'approve' || action.mode === 'edit'
          ? OnboardingActions.editAccount({ area: 'ResponsibleOfficer' })
          : OnboardingActions.navigateNext({
              mode: action.mode,
              next: '/organisation/register/contact-details',
            });
      })
    );
  });

  storeContactDetailsForCt$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.storeContactDetailsForCt),
      map((action) => {
        return action.mode === 'approve' || action.mode === 'edit'
          ? OnboardingActions.editAccount({ area: 'ContactDetailsForCt' })
          : OnboardingActions.navigateNext({
              mode: action.mode,
              next: '/organisation/register/check-answers',
            });
      })
    );
  });

  submitOnboardingDetails$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.submitOnboardingDetails),
      concatLatestFrom((_) =>
        this.store.select(selectForSubmitOnboardingDetails)
      ),
      exhaustMap(([_, dto]) => {
        return this.organisationService.saveOnboardingDetails(dto).pipe(
          map((_) => {
            return OnboardingActions.onSubmitOnboardingDetails();
          }),
          catchError((error: HttpErrorResponse) => {
            window.scroll(0, 0);
            return of(
              OnboardingActions.onError({ message: error.error.detail })
            );
          })
        );
      })
    );
  });

  onSubmitOnboardingDetails$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(OnboardingActions.onSubmitOnboardingDetails),
        map((_) => {
          this.router.navigate(['/organisation/register/confirmation']);
        })
      );
    },
    { dispatch: false }
  );

  storeAccountApproval$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(OnboardingActions.storeAccountApproval),
        concatLatestFrom((_) => this.store.select(selectOrganisationId)),
        map(([action, orgId]) => {
          let link = `/organisation/${orgId}/approve/confirm`;
          if (action.edit) {
            this.router.navigate([link], { queryParams: { edit: true } });
          } else {
            this.router.navigate([link]);
          }
        })
      );
    },
    { dispatch: false }
  );

  cancelAccountApproval$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(OnboardingActions.cancelAccountApproval),
        concatLatestFrom((_) => [
          this.store.select(selectOrganisationId),
          this.store.select(selectAccountApprovalFiles)
        ]),
        exhaustMap(([action, organisationId, files]) => {
          const observables: any = {};
          observables['default'] = of(0);
          for (const fileName of [ ... files.fileNames ]) {
            observables[fileName] = this.organisationService.deleteAccountApprovalFile(fileName, organisationId!);
          }
          return forkJoin(observables).pipe(
            map(_ => {
              const link = `/organisation/${organisationId}/edit/check-answers`;
              this.router.navigate([link], { queryParams: { mode: action.edit ? 'edit' : 'approve' } });
            })
          );
        })
      );
    }, { dispatch: false }
  );
  
  storeAccountRejection$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(OnboardingActions.storeAccountRejection),
        concatLatestFrom((_) => this.store.select(selectOrganisationId)),
        map(([action, orgId]) => {
          let link = `/organisation/${orgId}/reject/confirm`;
          if (action.edit) {
            this.router.navigate([link], { queryParams: { edit: true } });
          } else {
            this.router.navigate([link]);
          }
        })
      );
    },
    { dispatch: false }
  );

  cancelAccountRejection$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(OnboardingActions.cancelAccountRejection),
        concatLatestFrom((_) => [
          this.store.select(selectOrganisationId),
          this.store.select(selectAccountRejectionFiles)
        ]),
        exhaustMap(([action, organisationId, files]) => {
          const observables: any = {};
          observables['default'] = of(0);
          for (const fileName of [ ... files.fileNames ]) {
            observables[fileName] = this.organisationService.deleteAccountRejectionFile(fileName, organisationId!);
          }
          return forkJoin(observables).pipe(
            map(_ => {
              const link = `/organisation/${organisationId}/edit/check-answers`;
              this.router.navigate([link], { queryParams: { mode: action.edit ? 'edit' : 'approve' } });
            })
          );
        })
      );
    }, { dispatch: false }
  );

  getOrganisationDetails$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.getOrganisationDetails),
      exhaustMap((action) => {
        return this.organisationService.getManufacturer(action.orgId).pipe(
          switchMap((organisationDetails) => {
            this.sessionStorageService.setObject<Organisation>(
              navigationOrganisationSessionKey,
              {
                id: organisationDetails.id!,
                name: organisationDetails.responsibleUndertaking!.name,
                status: organisationDetails.status!,
              }
            );

            return [
              OnboardingActions.onGetOrganisationDetails({
                organisationDetails: organisationDetails,
                redirectToSummary: action.redirectToSummary,
              }),
              NavigationActions.toggleSecondaryNav({ show: true }),
            ];
          }),
          catchError((error) =>
            of(
              OnboardingActions.onErrorSaveOnboardingDetails({
                error,
              })
            )
          )
        );
      })
    );
  });

  onGetOrganisationDetails$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(OnboardingActions.onGetOrganisationDetails),
        concatLatestFrom((_) => this.store.select(selectSelectedSchemeYear)),
        map(([action, schemeYear]) => {
          if (
            action.organisationDetails.status === 'Active' &&
            action.redirectToSummary
          ) {
            this.router.navigate([
              `scheme-year/${schemeYear?.id}/organisation/${action.organisationDetails.id}/summary`,
            ]);
          }
        })
      );
    },
    { dispatch: false }
  );

  getAccountApprovalFiles$ = createEffect(() => {
      return this.actions$.pipe(
        ofType(OnboardingActions.getAccountApprovalFiles),
        switchMap((action) => {
          return this.organisationService
            .getAccountApprovalFiles(action.organisationId)
            .pipe(
              map((files) => {
                return OnboardingActions.onGetAccountApprovalFiles({
                  files: files,
                });
              }),
              catchError((errorResponse) =>
                of(
                  OnboardingActions.onErrorGetAccountApprovalFiles({
                    error: errorResponse,
                  })
                )
              )
            );
        })
      );
    });

    uploadAccountApprovalFile$ = createEffect(() => {
      return this.actions$.pipe(
        ofType(OnboardingActions.uploadAccountApprovalFile),
        exhaustMap((action) => {
          return this.organisationService
            .uploadAccountApprovalFiles(
              action.fileList,
              action.organisationId
            )
            .pipe(
              map((_) => {
                return OnboardingActions.getAccountApprovalFiles({
                  organisationId: action.organisationId
                });
              }),
              catchError((errorResponse: HttpErrorResponse) =>
                of(
                  OnboardingActions.onErrorUploadAccountApprovalFile({
                    error: errorResponse.error?.detail,
                  })
                )
              )
            );
        })
      );
    });
  
    deleteAccountApprovalFile$ = createEffect(() => {
      return this.actions$.pipe(
        ofType(OnboardingActions.deleteAccountApprovalFile),
        exhaustMap((action) => {
          return this.organisationService
            .deleteAccountApprovalFile(
              action.fileName,
              action.organisationId
            )
            .pipe(
              map((_) => {
                return OnboardingActions.getAccountApprovalFiles({
                  organisationId: action.organisationId
                });
              }),
              catchError((errorResponse: HttpErrorResponse) =>
                of(
                  OnboardingActions.onErrorDeleteAccountApprovalFile({
                    error: errorResponse.error?.detail,
                  })
                )
              )
            );
        })
      );
    });

  approveAccount$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.approveAccount),
      concatLatestFrom((_) =>
        this.store.select(selectForApproveOrganisationDto)
      ),
      exhaustMap(([_, command]) => {
        return this.organisationService.approveAccount(command).pipe(
          map((_) => {
            return OnboardingActions.onApproveAccount();
          }),
          catchError((error: HttpErrorResponse) =>
            of(OnboardingActions.onError({ message: error.error.detail }))
          )
        );
      })
    );
  });

  onApproveAccount$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(OnboardingActions.onApproveAccount),
        concatLatestFrom((_) => this.store.select(selectOrganisationId)),
        map(([_, orgId]) => {
          let link = `/organisation/${orgId}/approve/confirmation`;
          this.router.navigate([link]);
        })
      );
    },
    { dispatch: false }
  );
  
  getAccountRejectionFiles$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.getAccountRejectionFiles),
      switchMap((action) => {
        return this.organisationService
          .getAccountRejectionFiles(action.organisationId)
          .pipe(
            map((files) => {
              return OnboardingActions.onGetAccountRejectionFiles({
                files: files,
              });
            }),
            catchError((errorResponse) =>
              of(
                OnboardingActions.onErrorGetAccountRejectionFiles({
                  error: errorResponse,
                })
              )
            )
          );
      })
    );
  });

  uploadAccountRejectionFile$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.uploadAccountRejectionFile),
      exhaustMap((action) => {
        return this.organisationService
          .uploadAccountRejectionFiles(
            action.fileList,
            action.organisationId
          )
          .pipe(
            map((_) => {
              return OnboardingActions.getAccountRejectionFiles({
                organisationId: action.organisationId
              });
            }),
            catchError((errorResponse: HttpErrorResponse) =>
              of(
                OnboardingActions.onErrorUploadAccountRejectionFile({
                  error: errorResponse.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  deleteAccountRejectionFile$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.deleteAccountRejectionFile),
      exhaustMap((action) => {
        return this.organisationService
          .deleteAccountRejectionFile(
            action.fileName,
            action.organisationId
          )
          .pipe(
            map((_) => {
              return OnboardingActions.getAccountRejectionFiles({
                organisationId: action.organisationId
              });
            }),
            catchError((errorResponse: HttpErrorResponse) =>
              of(
                OnboardingActions.onErrorDeleteAccountRejectionFile({
                  error: errorResponse.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  rejectAccount$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.rejectAccount),
      concatLatestFrom((_) =>
        this.store.select(selectForRejectOrganisationDto)
      ),
      exhaustMap(([_, command]) => {
        return this.organisationService.rejectAccount(command).pipe(
          map((_) => {
            return OnboardingActions.onRejectAccount();
          }),
          catchError((error: HttpErrorResponse) =>
            of(OnboardingActions.onError({ message: error.error.detail }))
          )
        );
      })
    );
  });

  onRejectAccount$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(OnboardingActions.onRejectAccount),
        concatLatestFrom((_) => this.store.select(selectOrganisationId)),
        map(([_, orgId]) => {
          let link = `/organisation/${orgId}/reject/confirmation`;
          this.router.navigate([link]);
        })
      );
    },
    { dispatch: false }
  );

  editAccount$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.editAccount),
      concatLatestFrom((_) => this.store.select(selectForEditOrganisationDto)),
      exhaustMap(([action, command]) => {
        return this.organisationService.editAccount(action.area, command).pipe(
          map((_) => OnboardingActions.onEditAccount()),
          catchError((error) =>
            of(OnboardingActions.onErrorSaveOnboardingDetails({ error }))
          )
        );
      })
    );
  });

  onEditAccount$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(OnboardingActions.onEditAccount),
        concatLatestFrom((_) => this.store.select(selectOrgId)),
        map(([_, orgId]) => {
          let link = `/organisation/${orgId}/edit/check-answers`;
          this.router.navigate([link], {
            queryParams: { fetchData: true, mode: 'edit' },
          });
        })
      );
    },
    { dispatch: false }
  );

  navigateNext$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(OnboardingActions.navigateNext),
        concatLatestFrom((_) => this.store.select(selectOrganisationId)),
        map(([action, orgId]) => {
          let link = this.getNavigationLink(action.mode, action.next, orgId);
          this.router.navigate(link[0], link[1]);
        })
      );
    },
    { dispatch: false }
  );

  getNavigationLink(
    mode: OnboardingFormMode,
    next: string,
    orgId?: string | null
  ): [string[], NavigationExtras] {
    let link: string = next;
    let queryParams: Params | null = null;
    switch (mode) {
      case 'submit':
        link = '/organisation/register/check-answers';
        break;
      case 'approve':
        link = `/organisation/${orgId}/edit/check-answers'`;
        queryParams = { mode: mode };
        break;
      case 'edit':
        link = `/organisation/${orgId}/edit/check-answers'`;
        queryParams = { mode: mode };
        break;
    }
    return [[link], { queryParams: queryParams }];
  }

  clearIsSchemePartipantFormValue$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(OnboardingActions.clearIsSchemePartipantFormValue),
        map((action) => {
          this.router.navigate([
            `/organisation/${action.organisationId}/change-manufacturer-type`,
          ]);
        })
      );
    },
    { dispatch: false }
  );

  storeIsNonSchemePartipantFormValue$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(OnboardingActions.storeIsSchemePartipantFormValue),
        map((action) => {
          this.router.navigate([
            `/organisation/${action.organisationId}/change-manufacturer-type/check-answers`,
          ]);
        })
      );
    },
    { dispatch: false }
  );

  changeIsNonSchemeParticipant$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(OnboardingActions.changeIsNonSchemeParticipant),
      concatLatestFrom((_) => this.store.select(selectIsNonSchemePartipant)),
      exhaustMap(([action, isNonSchemePartipant]) => {
        return this.organisationService
          .editOrganisationSchemeParticipation({
            organisationId: action.organisationId,
            isNonSchemeParticipant: isNonSchemePartipant,
          })
          .pipe(
            map((_) => {
              return OnboardingActions.onChangeIsNonSchemeParticipant({
                organisationId: action.organisationId,
              });
            }),
            catchError((error) =>
              of(
                OnboardingActions.onChangeIsNonSchemeParticipantError({
                  message: error.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  onChangeIsNonSchemeParticipant$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(OnboardingActions.onChangeIsNonSchemeParticipant),
        map((action) => {
          this.router.navigate([
            `/organisation/${action.organisationId}/change-manufacturer-type/confirmation`,
          ]);
        })
      );
    },
    { dispatch: false }
  );
}
