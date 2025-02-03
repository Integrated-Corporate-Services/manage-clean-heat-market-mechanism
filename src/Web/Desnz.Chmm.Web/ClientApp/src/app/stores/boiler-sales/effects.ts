import { Actions, ofType, createEffect, concatLatestFrom } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { catchError, exhaustMap, map, switchMap } from 'rxjs/operators';
import * as BoilerSalesActions from './actions';
import { BoilerSalesService } from './services/boiler-sales-service';
import { Store } from '@ngrx/store';
import { of } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { selectQuarterlyBoilerSales, selectSubmitAnnual } from './selectors';
import { selectSelectedSchemeYear } from '../scheme-years/selectors';

@Injectable()
export class BoilerSalesEffects {
  constructor(
    private actions$: Actions,
    private store: Store,
    private boilerSalesService: BoilerSalesService,
    private router: Router
  ) {}

  getAnnualBoilerSales$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(BoilerSalesActions.getAnnualBoilerSales),
      concatLatestFrom((_) => this.store.select(selectSelectedSchemeYear)),
      exhaustMap(([action, schemeYear]) => {
        if (schemeYear === null) {
          throw Error('No scheme year selected');
        }
        return this.boilerSalesService
          .getAnnualBoilerSalesSummary(
            action.organisationId,
            action.schemeYearId
          )
          .pipe(
            map((annualBoilerSales) => {
              return BoilerSalesActions.onGetAnnualBoilerSales({
                annualBoilerSales,
                annualStatus: action.annualStatus,
                schemeYear,
              });
            }),
            catchError((error: HttpErrorResponse) =>
              of(BoilerSalesActions.onErrorGetAnnualBoilerSales({ error }))
            )
          );
      })
    );
  });

  approveAnnualBoilerSales$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(BoilerSalesActions.approveAnnualBoilerSales),
      exhaustMap((action) => {
        return this.boilerSalesService
          .approveAnnualBoilerSalesSummary(
            action.organisationId,
            action.schemeYearId
          )
          .pipe(
            map((_) => {
              return BoilerSalesActions.onApproveAnnualBoilerSales({
                organisationId: action.organisationId,
                schemeYearId: action.schemeYearId,
              });
            }),
            catchError((error: HttpErrorResponse) =>
              of(
                BoilerSalesActions.onApproveAnnualBoilerSalesError({
                  error: error.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  onApproveAnnualBoilerSales$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(BoilerSalesActions.onApproveAnnualBoilerSales),
        map((action) => {
          this.router.navigate([
            `/scheme-year/${action.schemeYearId}/organisation/${action.organisationId}/boiler-sales`,
          ]);
        })
      );
    },
    { dispatch: false }
  );

  copyAnnualBoilerSalesFilesForEditing$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(BoilerSalesActions.copyAnnualBoilerSalesFilesForEditing),
      switchMap((action) => {
        return this.boilerSalesService
          .copyAnnualBoilerSalesFilesForEditing(
            action.organisationId,
            action.schemeYearId
          )
          .pipe(
            switchMap((_) => {
              return of(
                BoilerSalesActions.onCopyAnnualBoilerSalesFilesForEditing(),
                BoilerSalesActions.getAnnualSupportingEvidence({
                  organisationId: action.organisationId,
                  schemeYearId: action.schemeYearId,
                  isEditing: true,
                }),
                BoilerSalesActions.getAnnualVerificationStatement({
                  organisationId: action.organisationId,
                  schemeYearId: action.schemeYearId,
                  isEditing: true,
                })
              );
            }),
            catchError((errorResponse: HttpErrorResponse) =>
              of(BoilerSalesActions.onCopyAnnualBoilerSalesFilesForEditing())
            )
          );
      })
    );
  });

  copyQuarterlyBoilerSalesFilesForEditing$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(BoilerSalesActions.copyQuarterlyBoilerSalesFilesForEditing),
      switchMap((action) => {
        return this.boilerSalesService
          .copyQuarterlyBoilerSalesFilesForEditing(
            action.organisationId,
            action.schemeYearId,
            action.schemeYearQuarterId
          )
          .pipe(
            switchMap((_) => {
              return of(
                BoilerSalesActions.onCopyQuarterlyBoilerSalesFilesForEditing(),
                BoilerSalesActions.getQuarterlySupportingEvidence({
                  organisationId: action.organisationId,
                  schemeYearId: action.schemeYearId,
                  schemeYearQuarterId: action.schemeYearQuarterId,
                  isEditing: true,
                })
              );
            }),
            catchError((errorResponse: HttpErrorResponse) =>
              of(BoilerSalesActions.onCopyQuarterlyBoilerSalesFilesForEditing())
            )
          );
      })
    );
  });

  getQuarterlyBoilerSales$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(BoilerSalesActions.getQuarterlyBoilerSales),
      concatLatestFrom((_) => this.store.select(selectSelectedSchemeYear)),
      exhaustMap(([action, schemeYear]) => {
        if (schemeYear === null) {
          throw Error('No scheme year selected');
        }

        return this.boilerSalesService
          .getQuarterlyBoilerSalesSummary(
            action.organisationId,
            action.schemeYearId
          )
          .pipe(
            map((quarterlyBoilerSales) => {
              return BoilerSalesActions.onGetQuarterlyBoilerSales({
                quarterlyBoilerSales,
                schemeYear,
              });
            }),
            catchError((error: HttpErrorResponse) =>
              of(BoilerSalesActions.onErrorGetQuarterlyBoilerSales({ error }))
            )
          );
      })
    );
  });

  getAnnualVerificationStatement$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(BoilerSalesActions.getAnnualVerificationStatement),
      switchMap((action) => {
        return this.boilerSalesService
          .getAnnualVerificationStatementFileNames(
            action.organisationId,
            action.schemeYearId,
            action.isEditing
          )
          .pipe(
            map((fileNames) => {
              return BoilerSalesActions.onGetAnnualVerificationStatement({
                fileNames,
              });
            }),
            catchError((errorResponse: HttpErrorResponse) =>
              of(
                BoilerSalesActions.onAnnualVerificationStatementError({
                  error: errorResponse.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  uploadAnnualVerificationStatement$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(BoilerSalesActions.uploadAnnualVerificationStatement),
      exhaustMap((action) => {
        return this.boilerSalesService
          .uploadAnnualVerificationStatement(
            action.fileList,
            action.organisationId,
            action.schemeYearId,
            action.isEditing
          )
          .pipe(
            map((_) => {
              return BoilerSalesActions.getAnnualVerificationStatement({
                organisationId: action.organisationId,
                schemeYearId: action.schemeYearId,
                isEditing: action.isEditing,
              });
            }),
            catchError((errorResponse: HttpErrorResponse) =>
              of(
                BoilerSalesActions.onAnnualVerificationStatementError({
                  error: errorResponse.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  deleteAnnualVerificationStatement$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(BoilerSalesActions.deleteAnnualVerificationStatement),
      exhaustMap((action) => {
        return this.boilerSalesService
          .deleteAnnualVerificationStatement(
            action.fileName,
            action.organisationId,
            action.schemeYearId,
            action.isEditing
          )
          .pipe(
            map((_) => {
              return BoilerSalesActions.getAnnualVerificationStatement({
                organisationId: action.organisationId,
                schemeYearId: action.schemeYearId,
                isEditing: action.isEditing,
              });
            }),
            catchError((errorResponse: HttpErrorResponse) =>
              of(
                BoilerSalesActions.onAnnualVerificationStatementError({
                  error: errorResponse.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  getAnnualSupportingEvidence$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(BoilerSalesActions.getAnnualSupportingEvidence),
      switchMap((action) => {
        return this.boilerSalesService
          .getAnnualSupportingEvidenceFileNames(
            action.organisationId,
            action.schemeYearId,
            action.isEditing
          )
          .pipe(
            map((fileNames) => {
              return BoilerSalesActions.onGetAnnualSupportingEvidence({
                fileNames,
              });
            }),
            catchError((errorResponse: HttpErrorResponse) =>
              of(
                BoilerSalesActions.onAnnualSupportingEvidenceError({
                  error: errorResponse.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  uploadAnnualSupportingEvidence$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(BoilerSalesActions.uploadAnnualSupportingEvidence),
      exhaustMap((action) => {
        return this.boilerSalesService
          .uploadAnnualSupportingEvidence(
            action.fileList,
            action.organisationId,
            action.schemeYearId,
            action.isEditing
          )
          .pipe(
            map((_) => {
              return BoilerSalesActions.getAnnualSupportingEvidence({
                organisationId: action.organisationId,
                schemeYearId: action.schemeYearId,
                isEditing: action.isEditing,
              });
            }),
            catchError((errorResponse: HttpErrorResponse) =>
              of(
                BoilerSalesActions.onAnnualSupportingEvidenceError({
                  error: errorResponse.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  deleteAnnualSupportingEvidence$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(BoilerSalesActions.deleteAnnualSupportingEvidence),
      exhaustMap((action) => {
        return this.boilerSalesService
          .deleteAnnualSupportingEvidence(
            action.fileName,
            action.organisationId,
            action.schemeYearId,
            action.isEditing
          )
          .pipe(
            map((_) => {
              return BoilerSalesActions.getAnnualSupportingEvidence({
                organisationId: action.organisationId,
                schemeYearId: action.schemeYearId,
                isEditing: action.isEditing,
              });
            }),
            catchError((errorResponse: HttpErrorResponse) =>
              of(
                BoilerSalesActions.onAnnualSupportingEvidenceError({
                  error: errorResponse.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  storeAnnualBoilerSales$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(BoilerSalesActions.storeAnnualBoilerSales),
        map((action) => {
          this.router.navigate([
            `/scheme-year/${action.schemeYearId}/organisation/${action.organisationId}/boiler-sales/annual/check-answers`,
          ]);
        })
      );
    },
    { dispatch: false }
  );

  submitAnnualBoilerSales$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(BoilerSalesActions.submitAnnualBoilerSales),
      concatLatestFrom((_) => this.store.select(selectSubmitAnnual)),
      exhaustMap(([action, boilerSales]) => {
        let command = {
          organisationId: action.organisationId,
          schemeYearId: action.schemeYearId,
          oil: Number(boilerSales.oil),
          gas: Number(boilerSales.gas),
        };
        let observable = boilerSales.isEditing
          ? this.boilerSalesService.editAnnualBoilerSales(command)
          : this.boilerSalesService.submitAnnualBoilerSales(command);
        return observable.pipe(
          map((_) => {
            return BoilerSalesActions.onSubmitAnnualBoilerSales({
              organisationId: action.organisationId,
              schemeYearId: action.schemeYearId,
            });
          }),
          catchError((error: HttpErrorResponse) => {
            return of(
              BoilerSalesActions.onSubmitAnnualBoilerSalesError({
                error: error.error?.detail,
              })
            );
          })
        );
      })
    );
  });

  onSubmitAnnualBoilerSales$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(BoilerSalesActions.onSubmitAnnualBoilerSales),
        map((action) => {
          this.router.navigate([
            `/scheme-year/${action.schemeYearId}/organisation/${action.organisationId}/boiler-sales/annual/confirmation`,
          ]);
        })
      );
    },
    { dispatch: false }
  );

  getQuarterlySupportingEvidence$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(BoilerSalesActions.getQuarterlySupportingEvidence),
      switchMap((action) => {
        return this.boilerSalesService
          .getQuarterlySupportingEvidenceFileNames(
            action.organisationId,
            action.schemeYearQuarterId,
            action.schemeYearId,
            action.isEditing
          )
          .pipe(
            map((fileNames) => {
              return BoilerSalesActions.onGetQuarterlySupportingEvidence({
                fileNames,
              });
            }),
            catchError((errorResponse: HttpErrorResponse) =>
              of(
                BoilerSalesActions.onGetQuarterlySupportingEvidenceError({
                  error: errorResponse.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  uploadQuarterlySupportingEvidence$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(BoilerSalesActions.uploadQuarterlySupportingEvidence),
      exhaustMap((action) => {
        return this.boilerSalesService
          .uploadQuarterlySupportingEvidence(
            action.fileList,
            action.organisationId,
            action.schemeYearQuarterId,
            action.schemeYearId,
            action.isEditing
          )
          .pipe(
            map((_) => {
              return BoilerSalesActions.getQuarterlySupportingEvidence({
                organisationId: action.organisationId,
                schemeYearQuarterId: action.schemeYearQuarterId,
                schemeYearId: action.schemeYearId,
                isEditing: action.isEditing
              });
            }),
            catchError((errorResponse: HttpErrorResponse) =>
              of(
                BoilerSalesActions.onGetQuarterlySupportingEvidenceError({
                  error: errorResponse.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  deleteQuarterlySupportingEvidence$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(BoilerSalesActions.deleteQuarterlySupportingEvidence),
      exhaustMap((action) => {
        return this.boilerSalesService
          .deleteQuarterlySupportingEvidence(
            action.fileName,
            action.organisationId,
            action.schemeYearQuarterId,
            action.schemeYearId,
            action.isEditing
          )
          .pipe(
            map((_) => {
              return BoilerSalesActions.getQuarterlySupportingEvidence({
                organisationId: action.organisationId,
                schemeYearQuarterId: action.schemeYearQuarterId,
                schemeYearId: action.schemeYearId,
                isEditing: action.isEditing
              });
            }),
            catchError((errorResponse: HttpErrorResponse) =>
              of(
                BoilerSalesActions.onGetQuarterlySupportingEvidenceError({
                  error: errorResponse.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  storeQuarterlyBoilerSales$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(BoilerSalesActions.storeQuarterlyBoilerSales),
        concatLatestFrom((_) => this.store.select(selectQuarterlyBoilerSales)),
        exhaustMap(([action, boilerSales]) => {
          let mode = boilerSales.isEditing ? 'edit' : 'submit';
          return this.router.navigate([
            `/scheme-year/${action.schemeYearId}/organisation/${action.organisationId}/boiler-sales/quarter/${action.schemeYearQuarterId}/${mode}/check-answers`,
          ]);
        })
      );
    },
    { dispatch: false }
  );

  submitQuarterlyBoilerSales$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(BoilerSalesActions.submitQuarterlyBoilerSales),
      concatLatestFrom((_) => this.store.select(selectQuarterlyBoilerSales)),
      exhaustMap(([action, boilerSales]) => {
        let command = {
          organisationId: action.organisationId,
          schemeYearId: action.schemeYearId,
          schemeYearQuarterId: action.schemeYearQuarterId,
          oil: Number(boilerSales.oil),
          gas: Number(boilerSales.gas),
        };
        let observable = boilerSales.isEditing
          ? this.boilerSalesService.editQuarterlyBoilerSales(command)
          : this.boilerSalesService.submitQuarterlyBoilerSales(command);
        return observable.pipe(
          map((_) => {
            return BoilerSalesActions.onSubmitQuarterlyBoilerSales({
              organisationId: action.organisationId,
              schemeYearId: action.schemeYearId,
              schemeYearQuarterId: action.schemeYearQuarterId
            });
          }),
          catchError((error: HttpErrorResponse) => {
            return of(
              BoilerSalesActions.onSubmitQuarterlyBoilerSalesError({
                error: error.error?.detail,
              })
            );
          })
        );
      })
    );
  });

  onSubmitQuarterlyBoilerSales$ = createEffect(
    () => {
      return this.actions$.pipe(
        ofType(BoilerSalesActions.onSubmitQuarterlyBoilerSales),
        concatLatestFrom((_) => this.store.select(selectQuarterlyBoilerSales)),
        exhaustMap(([action, boilerSales]) => {
          let mode = boilerSales.isEditing ? 'edit' : 'submit';
          return this.router.navigate([
            `/scheme-year/${action.schemeYearId}/organisation/${action.organisationId}/boiler-sales/quarter/${action.schemeYearQuarterId}/${mode}/confirmation`,
          ]);
        })
      );
    },
    { dispatch: false }
  );
}
