import { createReducer, on } from '@ngrx/store';
import { AccountManagementState } from './state';
import * as UserActions from './actions';
import { MultiFileUploadState } from '../../shared/store/MultiFileUploadState';

const defaultHttpState = {
  loading: false,
  errorMessage: null,
  data: null,
};

const defaultMultiFileUploadState: MultiFileUploadState = {
  files: null,
  fileNames: [],
  uploadingFiles: false,
  retrievingFiles: false,
  deletingFile: false,
  error: null,
};

const defaultState: AccountManagementState = {
  userBeingEdited: defaultHttpState,
  manufacturerUser: defaultHttpState,
  manufacturerUserBeingEdited: null,
  manufacturerUserBeingInvited: null,
  manufacturerUserBeingInvitedError: null,
  adminRoles: defaultHttpState,
  users: defaultHttpState,
  adminUsers: defaultHttpState,
  manufacturerUsers: defaultHttpState,
  manufacturers: defaultHttpState,
  manufacturerNotes: defaultHttpState,
  manufacturerExistingNoteFiles: {},
  manufacturerNewNoteFiles: defaultMultiFileUploadState,
};

export const userReducer = createReducer(
  defaultState,
  on(UserActions.getUsers, (state): AccountManagementState => {
    return {
      ...state,
      users: { ...defaultHttpState, loading: true },
    };
  }),
  on(UserActions.onGetUsers, (state, action): AccountManagementState => {
    return {
      ...state,
      users: {
        ...state.users,
        loading: false,
        data: [...action.users],
      },
    };
  }),
  on(UserActions.onErrorGetUsers, (state, action): AccountManagementState => {
    return {
      ...state,
      users: {
        ...state.users,
        loading: false,
        errorMessage: action.error,
      },
    };
  }),
  on(UserActions.getAdminUsers, (state): AccountManagementState => {
    return {
      ...state,
      adminUsers: { ...defaultHttpState, loading: true },
    };
  }),
  on(UserActions.onGetAdminUsers, (state, action): AccountManagementState => {
    return {
      ...state,
      adminUsers: {
        ...state.adminUsers,
        loading: false,
        data: [...action.users],
      },
    };
  }),
  on(
    UserActions.onErrorGetAdminUsers,
    (state, action): AccountManagementState => {
      return {
        ...state,
        adminUsers: {
          ...state.adminUsers,
          loading: false,
          errorMessage: action.error,
        },
      };
    }
  ),
  on(UserActions.getUser, (state): AccountManagementState => {
    return {
      ...state,
      users: { ...defaultHttpState, loading: true },
    };
  }),
  on(UserActions.onGetUser, (state, action): AccountManagementState => {
    return {
      ...state,
      userBeingEdited: {
        ...state.users,
        loading: false,
        data: { ...action.user },
      },
    };
  }),
  on(UserActions.onErrorGetUser, (state, action): AccountManagementState => {
    return {
      ...state,
      users: {
        ...state.users,
        loading: false,
        errorMessage: action.error,
      },
    };
  }),
  on(
    UserActions.inviteAdminUser,
    UserActions.updateUserDetails,
    UserActions.activateUser,
    UserActions.deactivateUser,
    (state): AccountManagementState => {
      return {
        ...state,
        userBeingEdited: {
          ...state.userBeingEdited,
          loading: true,
          errorMessage: null,
        },
      };
    }
  ),
  on(
    UserActions.onInviteAdminUser,
    UserActions.onUpdateUserDetails,
    UserActions.onUpdateManufacturerUser,
    UserActions.onActivateUser,
    UserActions.onDeactivateUser,
    (state): AccountManagementState => {
      return {
        ...state,
        userBeingEdited: {
          ...state.userBeingEdited,
          loading: false,
        },
      };
    }
  ),
  on(
    UserActions.onErrorInviteAdminUser,
    UserActions.onErrorUpdateUserDetails,
    UserActions.onErrorUpdateManufacturerUser,
    (state, action): AccountManagementState => {
      return {
        ...state,
        userBeingEdited: {
          ...state.userBeingEdited,
          loading: false,
          errorMessage: action.error,
        },
      };
    }
  ),
  on(
    UserActions.storeUserFormValue,
    (state, action): AccountManagementState => {
      let roles =
        state.adminRoles.data?.filter(
          (r) => r.id === action.userFormValue.roleId
        ) ?? [];
      return {
        ...state,
        userBeingEdited: {
          ...state.userBeingEdited,
          data: {
            ...state.userBeingEdited.data!,
            name: action.userFormValue.name,
            email: action.userFormValue.email,
            status: action.userFormValue.status,
            chmmRoles: roles,
          },
        },
      };
    }
  ),
  on(
    UserActions.clearUserBeingEdited,
    (state, action): AccountManagementState => {
      return {
        ...state,
        userBeingEdited: {
          ...defaultHttpState,
        },
      };
    }
  ),
  on(UserActions.getAdminRoles, (state): AccountManagementState => {
    return {
      ...state,
      adminRoles: { ...defaultHttpState, loading: true },
    };
  }),
  on(UserActions.onGetAdminRoles, (state, action): AccountManagementState => {
    return {
      ...state,
      adminRoles: {
        ...state.adminRoles,
        loading: false,
        data: action.roles != null ? [...action.roles] : [],
      },
    };
  }),
  on(UserActions.getManufacturerUsers, (state): AccountManagementState => {
    return {
      ...state,
      manufacturerUsers: { ...defaultHttpState, loading: true },
    };
  }),
  on(
    UserActions.getManufacturerUsersForMyOrganisation,
    (state): AccountManagementState => {
      return {
        ...state,
        manufacturerUsers: { ...defaultHttpState, loading: true },
      };
    }
  ),
  on(
    UserActions.onGetManufacturerUsers,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturerUsers: {
          ...state.manufacturerUsers,
          loading: false,
          data: [...action.users],
        },
      };
    }
  ),
  on(
    UserActions.onErrorGetManufacturerUsers,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturerUsers: {
          ...state.manufacturerUsers,
          loading: false,
          errorMessage: action.error,
        },
      };
    }
  ),
  on(UserActions.getManufacturers, (state): AccountManagementState => {
    return {
      ...state,
      manufacturers: { ...defaultHttpState, loading: true },
    };
  }),
  on(
    UserActions.onGetManufacturers,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturers: {
          ...state.manufacturers,
          loading: false,
          data: [...action.organisations],
        },
      };
    }
  ),
  on(
    UserActions.onErrorGetManufacturers,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturers: {
          ...state.manufacturers,
          loading: false,
          errorMessage: action.error,
        },
      };
    }
  ),
  on(UserActions.getManufacturerNotes, (state): AccountManagementState => {
    return {
      ...state,
      manufacturerNotes: { ...defaultHttpState, loading: true },
    };
  }),
  on(
    UserActions.onGetManufacturerNotes,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturerNotes: {
          ...state.manufacturerNotes,
          loading: false,
          data: action.notes,
        },
      };
    }
  ),
  on(
    UserActions.onErrorGetManufacturerNotes,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturerNotes: {
          ...state.manufacturerNotes,
          loading: false,
          errorMessage: action.error,
        },
      };
    }
  ),
  on(
    UserActions.onGetManufacturerExistingNoteFiles,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturerExistingNoteFiles: {
          ...state.manufacturerExistingNoteFiles,
          [action.noteId]: action.files,
        },
      };
    }
  ),
  on(
    UserActions.getManufacturerNewNoteFiles,
    (state): AccountManagementState => {
      return {
        ...state,
        manufacturerNewNoteFiles: {
          ...state.manufacturerNewNoteFiles,
          retrievingFiles: true,
          error: null,
        },
      };
    }
  ),
  on(
    UserActions.onGetManufacturerNewNoteFiles,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturerNewNoteFiles: {
          ...state.manufacturerNewNoteFiles,
          retrievingFiles: false,
          uploadingFiles: false,
          deletingFile: false,
          fileNames: action.files,
        },
      };
    }
  ),
  on(
    UserActions.uploadManufacturerNewNoteFile,
    (state): AccountManagementState => {
      return {
        ...state,
        manufacturerNewNoteFiles: {
          ...state.manufacturerNewNoteFiles,
          uploadingFiles: true,
          error: null,
        },
      };
    }
  ),
  on(
    UserActions.deleteManufacturerNewNoteFile,
    (state): AccountManagementState => {
      return {
        ...state,
        manufacturerNewNoteFiles: {
          ...state.manufacturerNewNoteFiles,
          deletingFile: true,
          error: null,
        },
      };
    }
  ),
  on(
    UserActions.onErrorUploadManufacturerNewNoteFile,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturerNewNoteFiles: {
          ...state.manufacturerNewNoteFiles,
          retrievingFiles: false,
          uploadingFiles: false,
          error: action.error,
        },
      };
    }
  ),
  on(
    UserActions.submitManufacturerNewNote,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturerNewNoteFiles: {
          ...state.manufacturerNewNoteFiles,
          loading: true,
          error: null,
        },
      };
    }
  ),
  on(
    UserActions.onSubmitManufacturerNewNote,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturerNewNoteFiles: {
          ...state.manufacturerNewNoteFiles,
          loading: false,
        },
      };
    }
  ),
  on(
    UserActions.storeManufacturerUserBeingInvited,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturerUserBeingInvited: { ...action.command },
      };
    }
  ),
  on(
    UserActions.clearManufacturerUserBeingInvited,
    (state): AccountManagementState => {
      return {
        ...state,
        manufacturerUserBeingInvited: null,
        manufacturerUserBeingInvitedError: null,
      };
    }
  ),
  on(
    UserActions.onErrorInviteManufacturerUser,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturerUserBeingInvitedError: action.error,
      };
    }
  ),
  on(UserActions.getManufacturerUser, (state): AccountManagementState => {
    return {
      ...state,
      manufacturerUser: { ...defaultHttpState, loading: true },
    };
  }),
  on(
    UserActions.onGetManufacturerUser,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturerUser: {
          ...state.manufacturerUser,
          loading: false,
          data: { ...action.user },
        },
      };
    }
  ),
  on(
    UserActions.onErrorGetManufacturerUser,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturerUser: {
          ...state.manufacturerUser,
          loading: false,
          errorMessage: action.error,
        },
      };
    }
  ),
  on(
    UserActions.editManufacturerUser,
    UserActions.updateManufacturerUser,
    UserActions.checkAnswersManufacturerUserBeingEdited,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturerUserBeingEdited: { ...action.command },
      };
    }
  ),
  on(
    UserActions.clearManufacturerUserBeingEdited,
    (state): AccountManagementState => {
      return {
        ...state,
        manufacturerUserBeingEdited: null,
      };
    }
  ),
  on(
    UserActions.onErrorDeactivateManufacturerUser,
    (state, action): AccountManagementState => {
      return {
        ...state,
        manufacturerUser: {
          ...state.manufacturerUser,
          loading: false,
          errorMessage: action.error,
        },
      };
    }
  )
);
