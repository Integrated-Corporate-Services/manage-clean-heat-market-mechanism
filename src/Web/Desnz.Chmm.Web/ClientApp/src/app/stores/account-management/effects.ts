import { Actions, ofType, createEffect, concatLatestFrom } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import {
  switchMap,
  map,
  catchError,
  exhaustMap,
  mergeMap,
} from 'rxjs/operators';
import { of } from 'rxjs';
import * as UserActions from './actions';
import { UserService } from './services/user-service';
import { RoleService } from 'src/app/stores/account-management/services/role.service';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import {
  selectManufacturerUserBeingInvited,
  selectUserBeingEdited,
  selectManufacturerUserBeingEdited
} from './selectors';
import { OrganisationService } from '../onboarding/services.ts/organisation-service';
import { selectOrganisationId } from '../auth/selectors';
import { NotesService } from './services/notes.service';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable()
export class UserEffects {
  constructor(
    private _actions$: Actions,
    private _userService: UserService,
    private _roleService: RoleService,
    private _router: Router,
    private _store: Store,
    private _organisationService: OrganisationService,
    private _notesService: NotesService
  ) {}

  getUser$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.getUser),
      switchMap((action) => {
        return this._userService.getAdminUser(action.userId).pipe(
          map((user) => {
            return UserActions.onGetUser({ user });
          }),
          catchError((errorResponse) =>
            of(UserActions.onErrorGetUser({ error: errorResponse }))
          )
        );
      })
    );
  });

  getUsers$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.getUsers),
      switchMap((_) => {
        return this._userService.getAllUsers().pipe(
          map((users) => {
            return UserActions.onGetUsers({
              users,
            });
          }),
          catchError((errorResponse) =>
            of(UserActions.onErrorGetUsers({ error: errorResponse }))
          )
        );
      })
    );
  });

  getAdminUsers$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.getAdminUsers),
      switchMap((_) => {
        return this._userService.getAdminUsers().pipe(
          map((users) => {
            return UserActions.onGetAdminUsers({
              users,
            });
          }),
          catchError((errorResponse) =>
            of(UserActions.onErrorGetUsers({ error: errorResponse }))
          )
        );
      })
    );
  });

  inviteAdminUser$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.inviteAdminUser),
      concatLatestFrom((_) => this._store.select(selectUserBeingEdited)),
      exhaustMap(([_, user]) => {
        let data = user.data!;
        return this._userService
          .inviteAdminUser({
            name: data.name,
            email: data.email,
            roleIds: [data.chmmRoles[0].id],
          })
          .pipe(
            map((_) => {
              return UserActions.onInviteAdminUser();
            }),
            catchError((errorResponse) =>
              of(UserActions.onErrorInviteAdminUser({ error: errorResponse }))
            )
          );
      })
    );
  });

  onInviteAdminUser$ = createEffect(
    () => {
      return this._actions$.pipe(
        ofType(UserActions.onInviteAdminUser),
        map((_) => {
          this._router.navigate(['/admin/users/invite/confirmation'], {
            queryParams: { mode: 'add' },
          });
        })
      );
    },
    { dispatch: false }
  );

  updateUserDetails$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.updateUserDetails),
      concatLatestFrom((_) => this._store.select(selectUserBeingEdited)),
      exhaustMap(([_, user]) => {
        let data = user.data!;
        return this._userService
          .editAdminUser({
            id: data.id,
            name: data.name,
            roleIds: [data.chmmRoles[0].id],
          })
          .pipe(
            map((_) => {
              return UserActions.onUpdateUserDetails();
            }),
            catchError((errorResponse) =>
              of(UserActions.onErrorInviteAdminUser({ error: errorResponse }))
            )
          );
      })
    );
  });

  updateManufacturerUserDetails$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.updateManufacturerUser),
      concatLatestFrom((_) => this._store.select(selectManufacturerUserBeingEdited)),
      exhaustMap(([_, user]) => {
        return this._userService
          .editManufacturerUser(user)
          .pipe(
            map((_) => {
              return UserActions.onUpdateManufacturerUser();
            }),
            catchError((errorResponse) =>
              of(UserActions.onErrorUpdateManufacturerUser({ error: errorResponse }))
            )
          );
      })
    );
  });

  onUpdateManufacturerUserDetails$ = createEffect(
    () => {
      return this._actions$.pipe(
        ofType(UserActions.onUpdateManufacturerUser),
        concatLatestFrom((_) => this._store.select(selectManufacturerUserBeingEdited)),
        map(([_, user]) => {
          this._router.navigate(
            [`/organisation/${user?.organisationId}/users/${user?.id}/confirmation`]
          );
        })
      );
    },
    { dispatch: false }
  );

  editManufacturerUserDetails$ = createEffect(
    () => {
      return this._actions$.pipe(
        ofType(UserActions.editManufacturerUser),
        concatLatestFrom((_) => this._store.select(selectManufacturerUserBeingEdited)),
        map(([_, user]) => {
          this._router.navigate(
            [`/organisation/${user?.organisationId}/users/${user?.id}/edit`]
          );
        })
      );
    },
    { dispatch: false }
  );

  onUpdateUserDetails$ = createEffect(
    () => {
      return this._actions$.pipe(
        ofType(UserActions.onUpdateUserDetails),
        concatLatestFrom((_) => this._store.select(selectUserBeingEdited)),
        map(([_, user]) => {
          this._router.navigate(
            [`/admin/users/edit/${user.data?.id}/confirmation`],
            {
              queryParams: { mode: 'edit' },
            }
          );
        })
      );
    },
    { dispatch: false }
  );

  storeUserFormValue$ = createEffect(
    () => {
      return this._actions$.pipe(
        ofType(UserActions.storeUserFormValue),
        concatLatestFrom((_) => this._store.select(selectUserBeingEdited)),
        map(([action, user]) => {
          this._router.navigate(
            [
              action.edit
                ? `/admin/users/edit/${user.data?.id}/check-answers`
                : '/admin/users/invite/check-answers',
            ],
            { queryParams: { edit: action.edit } }
          );
        })
      );
    },
    { dispatch: false }
  );

  getAdminRoles$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.getAdminRoles),
      switchMap((_) => {
        return this._roleService.getAdminRoles().pipe(
          map((roles) => {
            return UserActions.onGetAdminRoles({ roles });
          }),
          catchError((errorResponse) =>
            of(UserActions.onErrorGetUser({ error: errorResponse }))
          )
        );
      })
    );
  });

  activateUser$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.activateUser),
      concatLatestFrom((_) => this._store.select(selectUserBeingEdited)),
      switchMap(([_, user]) => {
        let data = user.data!;
        return this._userService.activateAdminUser({ id: data.id }).pipe(
          map((roles) => {
            return UserActions.onActivateUser();
          }),
          catchError((errorResponse) =>
            of(UserActions.onErrorGetUser({ error: errorResponse }))
          )
        );
      })
    );
  });

  deactivateUser$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.deactivateUser),
      concatLatestFrom((_) => this._store.select(selectUserBeingEdited)),
      switchMap(([_, user]) => {
        let data = user.data!;
        return this._userService.deactivateAdminUser({ id: data.id }).pipe(
          map((roles) => {
            return UserActions.onDeactivateUser();
          }),
          catchError((errorResponse) =>
            of(UserActions.onErrorGetUser({ error: errorResponse }))
          )
        );
      })
    );
  });

  onActivateUser$ = createEffect(
    () => {
      return this._actions$.pipe(
        ofType(UserActions.onActivateUser),
        concatLatestFrom((_) => this._store.select(selectUserBeingEdited)),
        map(([_, user]) => {
          this._router.navigate(
            [`/admin/users/edit/${user.data?.id}/confirmation`],
            {
              queryParams: { mode: 'activated' },
            }
          );
        })
      );
    },
    { dispatch: false }
  );

  onDeactivateUser$ = createEffect(
    () => {
      return this._actions$.pipe(
        ofType(UserActions.onDeactivateUser),
        concatLatestFrom((_) => this._store.select(selectUserBeingEdited)),
        map(([_, user]) => {
          this._router.navigate(
            [`/admin/users/edit/${user.data?.id}/confirmation`],
            {
              queryParams: { mode: 'deactivated' },
            }
          );
        })
      );
    },
    { dispatch: false }
  );

  getManufacturerUsersForMyOrganisation$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.getManufacturerUsersForMyOrganisation),
      concatLatestFrom((_) => this._store.select(selectOrganisationId)),
      switchMap(([_, organisationId]) => {
        return this._userService.getManufacturerUsers(organisationId).pipe(
          map((users) => {
            return UserActions.onGetManufacturerUsers({
              users,
            });
          }),
          catchError((errorResponse) =>
            of(
              UserActions.onErrorGetManufacturerUsers({ error: errorResponse })
            )
          )
        );
      })
    );
  });

  getManufacturerUsers$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.getManufacturerUsers),
      switchMap((action) => {
        return this._userService
          .getManufacturerUsers(action.organisationId)
          .pipe(
            map((users) => {
              return UserActions.onGetManufacturerUsers({
                users,
              });
            }),
            catchError((errorResponse) =>
              of(
                UserActions.onErrorGetManufacturerUsers({
                  error: errorResponse,
                })
              )
            )
          );
      })
    );
  });

  getManufacturers$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.getManufacturers),
      switchMap((_) => {
        return this._organisationService.getManufacturers().pipe(
          map((organisations) => {
            return UserActions.onGetManufacturers({
              organisations,
            });
          }),
          catchError((errorResponse) =>
            of(UserActions.onErrorGetManufacturers({ error: errorResponse }))
          )
        );
      })
    );
  });

  getManufacturerNotes$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.getManufacturerNotes),
      mergeMap((action) => {
        return this._notesService
          .getNotes(action.organisationId, action.schemeYearId)
          .pipe(
            mergeMap((notes) => {
              return [
                UserActions.onGetManufacturerNotes({ notes }),
                ...notes.map((note) =>
                  UserActions.getManufacturerExistingNoteFiles({
                    organisationId: action.organisationId,
                    schemeYearId: action.schemeYearId,
                    noteId: note.id,
                  })
                ),
              ];
            }),
            catchError((errorResponse) =>
              of(
                UserActions.onErrorGetManufacturerNotes({
                  error: errorResponse,
                })
              )
            )
          );
      })
    );
  });

  getManufacturerExistingNoteFiles$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.getManufacturerExistingNoteFiles),
      mergeMap((action) => {
        return this._notesService
          .getExistingNoteFileNames(
            action.organisationId,
            action.schemeYearId,
            action.noteId
          )
          .pipe(
            map((files) => {
              return UserActions.onGetManufacturerExistingNoteFiles({
                noteId: action.noteId,
                files: files,
              });
            }),
            catchError((errorResponse) =>
              of(
                UserActions.onErrorGetManufacturerExistingNoteFiles({
                  error: errorResponse,
                })
              )
            )
          );
      })
    );
  });

  getManufacturerNewNoteFiles$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.getManufacturerNewNoteFiles),
      switchMap((action) => {
        return this._notesService
          .getNewNoteFileNames(action.organisationId, action.schemeYearId)
          .pipe(
            map((files) => {
              return UserActions.onGetManufacturerNewNoteFiles({
                files: files,
              });
            }),
            catchError((errorResponse) =>
              of(
                UserActions.onErrorGetManufacturerNewNoteFiles({
                  error: errorResponse,
                })
              )
            )
          );
      })
    );
  });

  uploadManufacturerNewNoteFile$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.uploadManufacturerNewNoteFile),
      exhaustMap((action) => {
        return this._notesService
          .uploadNoteFile(
            action.fileList,
            action.organisationId,
            action.schemeYearId
          )
          .pipe(
            map((_) => {
              return UserActions.getManufacturerNewNoteFiles({
                organisationId: action.organisationId,
                schemeYearId: action.schemeYearId,
              });
            }),
            catchError((errorResponse: HttpErrorResponse) =>
              of(
                UserActions.onErrorUploadManufacturerNewNoteFile({
                  error: errorResponse.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  deleteManufacturerNewNoteFile$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.deleteManufacturerNewNoteFile),
      exhaustMap((action) => {
        return this._notesService
          .deleteNoteFile(
            action.fileName,
            action.organisationId,
            action.schemeYearId
          )
          .pipe(
            map((_) => {
              return UserActions.getManufacturerNewNoteFiles({
                organisationId: action.organisationId,
                schemeYearId: action.schemeYearId,
              });
            }),
            catchError((errorResponse: HttpErrorResponse) =>
              of(
                UserActions.onErrorUploadManufacturerNewNoteFile({
                  error: errorResponse.error?.detail,
                })
              )
            )
          );
      })
    );
  });

  submitManufacturerNewNote$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.submitManufacturerNewNote),
      exhaustMap((action) => {
        return this._notesService
          .submit(
            action.organisationId,
            action.schemeYearId,
            action.command.details
          )
          .pipe(
            map((_) => {
              return UserActions.onSubmitManufacturerNewNote({
                organisationId: action.organisationId,
                schemeYearId: action.schemeYearId,
              });
            }),
            catchError((error: HttpErrorResponse) => {
              return of(
                UserActions.onSubmitManufacturerNewNoteError({
                  error: error.error?.detail,
                })
              );
            })
          );
      })
    );
  });

  onSubmitManufacturerNewNote$ = createEffect(
    () => {
      return this._actions$.pipe(
        ofType(UserActions.onSubmitManufacturerNewNote),
        map((action) => {
          this._router.navigate([
            `/scheme-year/${action.schemeYearId}/organisation/${action.organisationId}/notes`,
          ]);
        })
      );
    },
    { dispatch: false }
  );

  clearManufacturerNewNoteFiles$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.clearManufacturerNewNoteFiles),
      exhaustMap((action) => {
        return this._notesService
          .clearManufacturerNewNoteFiles(
            action.organisationId,
            action.schemeYearId
          )
          .pipe(
            map((_) => {
              return UserActions.onClearManufacturerNewNoteFiles({
                organisationId: action.organisationId,
              });
            })
          );
      })
    );
  });

  inviteManufacturerUser$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.inviteManufacturerUser),
      concatLatestFrom((_) =>
        this._store.select(selectManufacturerUserBeingInvited)
      ),
      exhaustMap(([action, command]) => {
        return this._userService.inviteManufacturerUser(command!).pipe(
          map((_) => {
            this._router.navigate([
              `/organisation/${command!.organisationId}/users/invited`,
            ]);
            return UserActions.onErrorInviteManufacturerUser({ error: null });
          }),
          catchError((error: HttpErrorResponse) => {
            this._router.navigate([
              `/organisation/${command!.organisationId}/users/invite`,
            ]);
            return of(
              UserActions.onErrorInviteManufacturerUser({
                error: error.error?.detail,
              })
            );
          })
        );
      })
    );
  });

  getManufacturerUser$ = createEffect(() => {
    return this._actions$.pipe(
      ofType(UserActions.getManufacturerUser),
      switchMap((action) => {
        return this._userService
          .getManufacturerUser(action.organisationId, action.userId)
          .pipe(
            map((user) => {
              return UserActions.onGetManufacturerUser({ user });
            }),
            catchError((errorResponse) =>
              of(
                UserActions.onErrorGetManufacturerUser({ error: errorResponse })
              )
            )
          );
      })
    );
  });

  deactivateManufacturerUser$ = createEffect(
    () => {
      return this._actions$.pipe(
        ofType(UserActions.deactivateManufacturerUser),
        exhaustMap((action) => {
          return this._userService
            .deactivateManufacturerUser(action.userId, action.organisationId)
            .pipe(
              map((_) => {
                this._router.navigate([
                  `/organisation/${action.organisationId}/users/${action.userId}/deactivated`,
                ]);
              }),
              catchError((errorResponse) =>
                of(
                  UserActions.onErrorDeactivateManufacturerUser({
                    error: errorResponse,
                  })
                )
              )
            );
        })
      );
    },
    { dispatch: false }
  );
}
