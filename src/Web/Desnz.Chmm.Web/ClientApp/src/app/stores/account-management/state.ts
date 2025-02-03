import { MultiFileUploadState } from '../../shared/store/MultiFileUploadState';
import { HttpState } from '../http-state';
import { ViewOrganisationDto } from '../onboarding/dtos/view-organisation-dto';
import { EditManufacturerUserCommand } from './commands/edit-manufacturer-user-command';
import { InviteManufacturerUserCommand } from './commands/invite-manufacturer-user-command';
import { ChmmRole } from './dtos/chmm-role';
import { ChmmUser } from './dtos/chmm-user';
import { ManufacturerNote } from './dtos/manufacturer-note';

export interface AccountManagementState {
  users: HttpState<ChmmUser[]>;
  adminUsers: HttpState<ChmmUser[]>;
  manufacturerUser: HttpState<ChmmUser>;
  manufacturerUserBeingEdited: EditManufacturerUserCommand | null;
  manufacturerUserBeingInvited: InviteManufacturerUserCommand | null;
  manufacturerUserBeingInvitedError: string | null;
  userBeingEdited: HttpState<ChmmUser | null>;
  adminRoles: HttpState<ChmmRole[]>;
  manufacturerUsers: HttpState<ChmmUser[]>;
  manufacturers: HttpState<ViewOrganisationDto[]>;
  manufacturerNotes: HttpState<ManufacturerNote[]>;
  manufacturerExistingNoteFiles: Record<string, string[]>;
  manufacturerNewNoteFiles: MultiFileUploadState;
}
