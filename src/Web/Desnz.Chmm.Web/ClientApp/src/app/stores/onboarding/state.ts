import { OrganisationDetails } from 'src/app/manufacturer/onboarding/models/organisation-details';
import { HttpState } from '../http-state';
import { AccountApproval } from 'src/app/admin/approve-manufacturer/models/account-approval';
import { isSchemeParticipant } from 'src/app/admin/manufacturers/change-is-non-scheme-participant/change-is-non-scheme-participant-form/change-is-non-scheme-participant-form.component';
import { MultiFileUploadState } from 'src/app/shared/store/MultiFileUploadState';
import { AccountRejection } from 'src/app/admin/reject-manufacturer/models/account-rejection';

export interface OrganisationState {
  submitOnboarding: HttpState;
  organisationDetails: HttpState<OrganisationDetails>;
  accountApproval: AccountApproval | null;
  accountApprovalFiles: MultiFileUploadState;
  accountRejection: AccountRejection | null;
  accountRejectionFiles: MultiFileUploadState;
  changeIsNonSchemeParticipant: HttpState;
  isSchemePartipantFormValue: isSchemeParticipant | null;
}
