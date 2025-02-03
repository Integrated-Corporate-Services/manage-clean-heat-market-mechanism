import { createSelector } from '@ngrx/store';
import { AccountManagementState } from './state';
import { AppState } from '..';
import { MultiFileUploadState } from '../../shared/store/MultiFileUploadState';

export const selectUserState = (state: AppState) => state.accountManagement;

export const selectUsers = createSelector(
  selectUserState,
  (state: AccountManagementState) => state.users
);

export const selectAdminUsers = createSelector(
  selectUserState,
  (state: AccountManagementState) => state.adminUsers
);

export const selectUserBeingEdited = createSelector(
  selectUserState,
  (state: AccountManagementState) => state.userBeingEdited
);

export const selectRoles = createSelector(
  selectUserState,
  (state: AccountManagementState) => state.adminRoles
);

export const selectManufacturerUsers = createSelector(
  selectUserState,
  (state: AccountManagementState) => state.manufacturerUsers
);

export const selectManufacturers = createSelector(
  selectUserState,
  (state: AccountManagementState) => state.manufacturers
);

export const selectManufacturerNotes = createSelector(
  selectUserState,
  (state: AccountManagementState) => state.manufacturerNotes
);

export const selectManufacturerExistingNoteFiles = createSelector(
  selectUserState,
  (state: AccountManagementState) => state.manufacturerExistingNoteFiles
);

export const selectManufacturerNewNoteFiles = createSelector(
  selectUserState,
  (state: AccountManagementState) => state.manufacturerNewNoteFiles
);

export const selectAddManufacturerNoteLoading = createSelector(
  selectManufacturerNewNoteFiles,
  (files: MultiFileUploadState) => {
    return (
      files.uploadingFiles ||
      files.deletingFile ||
      files.retrievingFiles ||
      files.loading
    );
  }
);

export const selectManufacturerUserBeingInvited = createSelector(
  selectUserState,
  (state: AccountManagementState) => state.manufacturerUserBeingInvited
);

export const selectManufacturerUserBeingInvitedError = createSelector(
  selectUserState,
  (state: AccountManagementState) => state.manufacturerUserBeingInvitedError
);

export const selectManufacturerUser = createSelector(
  selectUserState,
  (state: AccountManagementState) => state.manufacturerUser
);

export const selectManufacturerUserBeingEdited = createSelector(
  selectUserState,
  (state: AccountManagementState) => state.manufacturerUserBeingEdited
);
