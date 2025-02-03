import { createAction, props } from '@ngrx/store';
import { AccountApproval } from 'src/app/admin/approve-manufacturer/models/account-approval';
import { Address } from 'src/app/manufacturer/onboarding/models/address';
import { CreditContactDetails } from 'src/app/manufacturer/onboarding/models/contact-details';
import { HeatPumps } from 'src/app/manufacturer/onboarding/models/heat-pump-brands';
import { OnboardingFormMode } from 'src/app/manufacturer/onboarding/models/mode';
import { ResponsibleUndertaking } from 'src/app/manufacturer/onboarding/models/responsible-undertaking';
import { UserDetails } from 'src/app/manufacturer/onboarding/models/user-details';
import { OrganisationStructure } from 'src/app/manufacturer/onboarding/models/organisation-structure';
import { GetEditableOrganisationDto } from './dtos/get-editable-organisation-dto';
import { isSchemeParticipant } from 'src/app/admin/manufacturers/change-is-non-scheme-participant/change-is-non-scheme-participant-form/change-is-non-scheme-participant-form.component';
import { AccountRejection } from 'src/app/admin/reject-manufacturer/models/account-rejection';

export const storeIsOnBehalfOfGroup = createAction(
  '[Onboarding] Store on behalf of group',
  props<{
    organisationStructure: OrganisationStructure;
    mode: OnboardingFormMode;
  }>()
);

export const storeResponsibleUndertaking = createAction(
  '[Onboarding] Store responsible undertaking',
  props<{
    responsibleUndertaking: ResponsibleUndertaking;
    mode: OnboardingFormMode;
  }>()
);

export const storeAddress = createAction(
  '[Onboarding] Store address form value',
  props<{ address: Address; mode: OnboardingFormMode }>()
);

export const storeLegalCorrespondenceAddress = createAction(
  '[Onboarding] Store legal correspondence address form value',
  props<{ mode: OnboardingFormMode; address: Partial<Address> }>()
);

export const storeIsNonSchemeParticipant = createAction(
  '[Onboarding] Store is non scheme participant',
  props<{ mode: OnboardingFormMode; isNonSchemeParticipant: boolean }>()
);

export const storeIsFossilFuelBoilerSeller = createAction(
  '[Onboarding] Store sell fossil fuel boilers',
  props<{ isFossilFuelBoilerSeller: string; mode: OnboardingFormMode }>()
);

export const storeHeatPumpBrands = createAction(
  '[Onboarding] Store heat pump brands',
  props<{ heatPumps: HeatPumps; mode: OnboardingFormMode }>()
);

export const storeUserDetails = createAction(
  '[Onboarding] Store user details',
  props<{ userDetails: UserDetails; mode: OnboardingFormMode }>()
);

export const storeResponsibleOfficer = createAction(
  '[Onboarding] Store responsible officer',
  props<{ mode: OnboardingFormMode; userDetails: Partial<UserDetails> }>()
);

export const storeContactDetailsForCt = createAction(
  '[Onboarding] Store contact details for credit transfers',
  (
    mode: OnboardingFormMode,
    contactDetails: CreditContactDetails | null = null
  ) => ({
    contactDetails: contactDetails,
    mode: mode,
  })
);

export const submitOnboardingDetails = createAction(
  '[Onboarding] submit onboarding details'
);

export const onSubmitOnboardingDetails = createAction(
  '[Onboarding] On: Submit onboarding details'
);

export const onErrorSaveOnboardingDetails = createAction(
  '[Account Management] On Error: Save onboarding details',
  props<{ error: any }>()
);

export const storeAccountApproval = createAction(
  '[Approve Manufacturer] Store account approval',
  props<{ accountApproval: AccountApproval; edit: boolean }>()
);

export const cancelAccountApproval = createAction(
  '[Approve Manufacturer] Cancel account approval',
  props<{ edit: boolean }>()
);

export const getAccountApprovalFiles = createAction(
  '[Approve Manufacturer] Get account approval files',
  props<{ organisationId: string }>()
);

export const onGetAccountApprovalFiles = createAction(
  '[Approve Manufacturer] On: Get account approval files',
  props<{ files: string[] }>()
);

export const onErrorGetAccountApprovalFiles = createAction(
  '[Approve Manufacturer] On Error: Get account approval files',
  props<{ error: any }>()
);

export const uploadAccountApprovalFile = createAction(
  '[Approve Manufacturer] Upload account approval files',
  props<{ organisationId: string; fileList: FileList }>()
);

export const onErrorUploadAccountApprovalFile = createAction(
  '[Approve Manufacturer] On Error: Upload account approval files',
  props<{ error?: string | null }>()
);

export const deleteAccountApprovalFile = createAction(
  '[Approve Manufacturer] Delete account approval file',
  props<{ organisationId: string; fileName: string }>()
);

export const onErrorDeleteAccountApprovalFile = createAction(
  '[Approve Manufacturer] On Error: Delete account approval files',
  props<{ error?: string | null }>()
);

export const approveAccount = createAction(
  '[Approve Manufacturer] Approve account'
);

export const onApproveAccount = createAction(
  '[Approve Manufacturer] On: Approve account'
);

export const cancelAccountRejection = createAction(
  '[Reject Manufacturer] Cancel account rejection',
  props<{ edit: boolean }>()
);

export const storeAccountRejection = createAction(
  '[Reject Manufacturer] Store account rejection',
  props<{ accountRejection: AccountRejection; edit: boolean }>()
);

export const getAccountRejectionFiles = createAction(
  '[Reject Manufacturer] Get account rejection files',
  props<{ organisationId: string }>()
);

export const onGetAccountRejectionFiles = createAction(
  '[Reject Manufacturer] On: Get account rejection files',
  props<{ files: string[] }>()
);

export const onErrorGetAccountRejectionFiles = createAction(
  '[Reject Manufacturer] On Error: Get account rejection files',
  props<{ error: any }>()
);

export const uploadAccountRejectionFile = createAction(
  '[Reject Manufacturer] Upload account rejection files',
  props<{ organisationId: string; fileList: FileList }>()
);

export const onErrorUploadAccountRejectionFile = createAction(
  '[Reject Manufacturer] On Error: Upload account rejection files',
  props<{ error?: string | null }>()
);

export const deleteAccountRejectionFile = createAction(
  '[Reject Manufacturer] Delete account rejection file',
  props<{ organisationId: string; fileName: string }>()
);

export const onErrorDeleteAccountRejectionFile = createAction(
  '[Reject Manufacturer] On Error: Delete account rejection files',
  props<{ error?: string | null }>()
);

export const rejectAccount = createAction(
  '[Reject Manufacturer] Reject account'
);

export const onRejectAccount = createAction(
  '[Reject Manufacturer] On: Reject account'
);

export type EditAccountArea =
  | '*'
  | 'IsOnBehalfOfGroup'
  | 'ResponsibleUndertaking'
  | 'Address'
  | 'LegalCorrespondenceAddress'
  | 'IsNonSchemeParticipant'
  | 'IsFossilFuelBoilerSeller'
  | 'HeatPumpBrands'
  | 'UserDetails'
  | 'ResponsibleOfficer'
  | 'ContactDetailsForCt';

export const editAccount = createAction(
  '[Approve Manufacturer] Edit account',
  props<{ area: EditAccountArea }>()
);

export const onEditAccount = createAction(
  '[Approve Manufacturer] On: Edit account'
);

export const getOrganisationDetails = createAction(
  '[Approve Manufacturer] Get organisation details',
  props<{
    orgId: string;
    redirectToSummary: boolean;
  }>()
);

export const onGetOrganisationDetails = createAction(
  '[Approve Manufacturer] On: Get organisation details',
  props<{
    organisationDetails: GetEditableOrganisationDto;
    redirectToSummary: boolean;
  }>()
);

export const onError = createAction(
  '[Approve Manufacturer] On HTTP request error',
  props<{ message: string }>()
);

export const navigateNext = createAction(
  '[Onboarding] Navigate next',
  props<{ mode: OnboardingFormMode; next: string; orgId?: string | null }>()
);

export const storeIsSchemePartipantFormValue = createAction(
  '[Change is non-scheme partipant] Store is scheme partipant form value',
  props<{
    organisationId: string;
    isSchemeParticipant: isSchemeParticipant;
  }>()
);

export const changeIsNonSchemeParticipant = createAction(
  '[Change is non-scheme partipant] Change is non-scheme partipant',
  props<{ organisationId: string }>()
);

export const onChangeIsNonSchemeParticipant = createAction(
  '[Change is non-scheme partipant] On: Change is non-scheme partipant',
  props<{ organisationId: string }>()
);

export const onChangeIsNonSchemeParticipantError = createAction(
  '[Change is non-scheme partipant] On Error: Change is non-scheme partipant',
  props<{ message: string }>()
);

export const clearIsSchemePartipantFormValue = createAction(
  '[Change is non-scheme partipant] Clear is scheme partipant form value',
  props<{ organisationId: string }>()
);
