import { createAction, props } from '@ngrx/store';
import { ChmmUser } from './dtos/chmm-user';
import { ChmmRole } from './dtos/chmm-role';
import { UserFormValue } from 'src/app/admin/account-management/models/user-form-value';
import { ViewOrganisationDto } from '../onboarding/dtos/view-organisation-dto';
import { ManufacturerNote } from './dtos/manufacturer-note';
import { AddManufacturerNoteCommand } from './commands/add-manufacturer-note-command';
import { InviteManufacturerUserCommand } from './commands/invite-manufacturer-user-command';
import { EditManufacturerUserCommand } from './commands/edit-manufacturer-user-command';

export const getUsers = createAction('[Account Management] Get users');

export const onGetUsers = createAction(
  '[Account Management] On: Get users',
  props<{ users: ChmmUser[] }>()
);

export const onErrorGetUsers = createAction(
  '[Account Management] On Error: Get users',
  props<{ error: any }>()
);

export const getAdminUsers = createAction(
  '[Account Management] Get admin users'
);

export const onGetAdminUsers = createAction(
  '[Account Management] On: Get admin users',
  props<{ users: ChmmUser[] }>()
);

export const onErrorGetAdminUsers = createAction(
  '[Account Management] On Error: Get admin users',
  props<{ error: any }>()
);

export const getUser = createAction(
  '[Account Management] Get user',
  props<{ userId: string }>()
);

export const onGetUser = createAction(
  '[Account Management] On: Get user',
  props<{ user: ChmmUser }>()
);

export const onErrorGetUser = createAction(
  '[Account Management] On Error: Get user',
  props<{ error: any }>()
);

export const inviteAdminUser = createAction(
  '[Account Management] Invite admin user'
);

export const onInviteAdminUser = createAction(
  '[Account Management] On: Invite admin user'
);

export const onErrorInviteAdminUser = createAction(
  '[Account Management] On Error: Invite admin user',
  props<{ error: any }>()
);

export const updateUserDetails = createAction(
  '[Account Management] Update user details'
);

export const onUpdateUserDetails = createAction(
  '[Account Management] On update user details'
);

export const onErrorUpdateUserDetails = createAction(
  '[User] On Error: Update user details',
  props<{ error: any }>()
);

export const storeUserFormValue = createAction(
  '[Account Management] Store user form value',
  props<{
    userFormValue: UserFormValue;
    edit: boolean;
  }>()
);

export const getAdminRoles = createAction(
  '[Account Management] Get admin roles'
);

export const onGetAdminRoles = createAction(
  '[Account Management] On: Get admin roles',
  props<{ roles: ChmmRole[] }>()
);

export const clearUserBeingEdited = createAction(
  '[Account Management] Clear user being edited'
);

export const activateUser = createAction('[Account Management] Activate user');

export const onActivateUser = createAction(
  '[Account Management] On activate user'
);

export const deactivateUser = createAction(
  '[Account Management] Deactivate user'
);

export const onDeactivateUser = createAction(
  '[Account Management] On deactivate user'
);

export const getManufacturerUsers = createAction(
  '[Account Management] Get manufacturer users',
  props<{ organisationId: string }>()
);

export const getManufacturerUsersForMyOrganisation = createAction(
  '[Account Management] Get manufacturer users for my organisation'
);

export const onGetManufacturerUsers = createAction(
  '[Account Management] On: Get manufacturer users',
  props<{ users: ChmmUser[] }>()
);

export const onErrorGetManufacturerUsers = createAction(
  '[Account Management] On Error: Get manufacturer users',
  props<{ error: any }>()
);

export const getManufacturers = createAction(
  '[Account Management] Get manufacturers'
);

export const onGetManufacturers = createAction(
  '[Account Management] On: Get manufacturers',
  props<{ organisations: ViewOrganisationDto[] }>()
);

export const onErrorGetManufacturers = createAction(
  '[Account Management] On Error: Get manufacturers',
  props<{ error: any }>()
);

export const getManufacturerNotes = createAction(
  '[Account Management] Get manufacturer notes',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onGetManufacturerNotes = createAction(
  '[Account Management] On: Get manufacturer notes',
  props<{ notes: ManufacturerNote[] }>()
);

export const onErrorGetManufacturerNotes = createAction(
  '[Account Management] On Error: Get manufacturer notes',
  props<{ error: any }>()
);

export const getManufacturerExistingNoteFiles = createAction(
  '[Account Management] Get manufacturer existing note files',
  props<{ organisationId: string; schemeYearId: string; noteId: string }>()
);

export const onGetManufacturerExistingNoteFiles = createAction(
  '[Account Management] On: Get manufacturer existing note files',
  props<{ noteId: string; files: string[] }>()
);

export const onErrorGetManufacturerExistingNoteFiles = createAction(
  '[Account Management] On Error: Get manufacturer existing note files',
  props<{ error: any }>()
);

export const getManufacturerNewNoteFiles = createAction(
  '[Account Management] Get manufacturer new note files',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onGetManufacturerNewNoteFiles = createAction(
  '[Account Management] On: Get manufacturer new note files',
  props<{ files: string[] }>()
);

export const onErrorGetManufacturerNewNoteFiles = createAction(
  '[Account Management] On Error: Get manufacturer new note files',
  props<{ error: any }>()
);

export const uploadManufacturerNewNoteFile = createAction(
  '[Account Management] Upload manufacturer new note files',
  props<{ organisationId: string; schemeYearId: string; fileList: FileList }>()
);

export const onErrorUploadManufacturerNewNoteFile = createAction(
  '[Account Management] On Error: Upload manufacturer new note files',
  props<{ error?: string | null }>()
);

export const deleteManufacturerNewNoteFile = createAction(
  '[Account Management] Delete manufacturer new note file',
  props<{ organisationId: string; schemeYearId: string; fileName: string }>()
);

export const submitManufacturerNewNote = createAction(
  '[Account Management] Submit manufacturer new note',
  props<{
    organisationId: string;
    schemeYearId: string;
    command: AddManufacturerNoteCommand;
  }>()
);

export const onSubmitManufacturerNewNote = createAction(
  '[Account Management] On: Submit manufacturer new note',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onSubmitManufacturerNewNoteError = createAction(
  '[Account Management] On Error: Submit manufacturer new note',
  props<{ error?: string | null }>()
);

export const clearManufacturerNewNoteFiles = createAction(
  '[Account Management] Clear manufacturer new note files',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onClearManufacturerNewNoteFiles = createAction(
  '[Account Management] On: Clear manufacturer new note files',
  props<{ organisationId: string }>()
);

export const storeManufacturerUserBeingInvited = createAction(
  '[Account Management] Store manufacturer user being invited',
  props<{ command: InviteManufacturerUserCommand }>()
);

export const clearManufacturerUserBeingInvited = createAction(
  '[Account Management] Clear manufacturer user being invited'
);

export const inviteManufacturerUser = createAction(
  '[Account Management] Invite manufacturer user'
);

export const onErrorInviteManufacturerUser = createAction(
  '[Account Management] On Error: Invite manufacturer user',
  props<{ error: string | null }>()
);

export const getManufacturerUser = createAction(
  '[Account Management] Get manufacturer user',
  props<{ organisationId: string; userId: string }>()
);

export const onGetManufacturerUser = createAction(
  '[Account Management] On: Get manufacturer user',
  props<{ user: ChmmUser }>()
);

export const onErrorGetManufacturerUser = createAction(
  '[Account Management] On Error: Get manufacturer user',
  props<{ error: any }>()
);

export const checkAnswersManufacturerUserBeingEdited = createAction(
  '[Account Management] Check answers manufacturer user being edited',
  props<{ command: EditManufacturerUserCommand }>()
);

export const editManufacturerUser = createAction(
  '[Account Management] edit manufacturer user',
  props<{ command: EditManufacturerUserCommand }>()
);

export const updateManufacturerUser = createAction(
  '[Account Management] Store manufacturer user being edited',
  props<{ command: EditManufacturerUserCommand }>()
);

export const onUpdateManufacturerUser = createAction(
  '[Account Management] On: Store manufacturer user being edited',
);

export const onErrorUpdateManufacturerUser = createAction(
  '[Account Management] On Error: Store manufacturer user being edited',
  props<{ error: any }>()
);

export const clearManufacturerUserBeingEdited = createAction(
  '[Account Management] Clear manufacturer user being edited'
);

export const deactivateManufacturerUser = createAction(
  '[Account Management] Deactivate manufacturer user',
  props<{ organisationId: string; userId: string }>()
);

export const onErrorDeactivateManufacturerUser = createAction(
  '[Account Management] On Error: Deactivate manufacturer user',
  props<{ error: any }>()
);
