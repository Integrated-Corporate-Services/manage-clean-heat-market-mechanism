import { createSelector } from '@ngrx/store';
import { OrganisationState } from './state';
import { AppState } from '..';
import { Address } from 'src/app/manufacturer/onboarding/models/address';
import { UserDetails } from 'src/app/manufacturer/onboarding/models/user-details';
import {
  toApproveManufacturerOrganisationCommand,
  toEditManufacturerOrganisationCommand,
  toOnboardManufacturerCommand,
  toRejectManufacturerOrganisationCommand,
} from './utils';
import { OrganisationDetails } from 'src/app/manufacturer/onboarding/models/organisation-details';
import { MultiFileUploadState } from 'src/app/shared/store/MultiFileUploadState';

export const selectOrganisationState = (state: AppState) => state.onboarding;

export const selectOrganisationDetailsLoading = (state: AppState) =>
  state.onboarding.organisationDetails.loading;

export const selectOrganisationId = (state: AppState) =>
  state.onboarding.organisationDetails.data?.id;

export const selectOrganisationDetails = (state: AppState) =>
  state.onboarding.organisationDetails;

export const selectOrganisationDetailsData = (state: AppState) =>
  state.onboarding.organisationDetails.data!;

export const selectOrgId = createSelector(
  selectOrganisationDetailsData,
  (state: OrganisationDetails) => state.id
);

export const selectAddress = createSelector(
  selectOrganisationDetailsData,
  (state: OrganisationDetails) => state.address
);

export const selectContactDetails = createSelector(
  selectOrganisationDetailsData,
  (state: OrganisationDetails) => state.creditContactDetails
);

export const selectHeatPumps = createSelector(
  selectOrganisationDetailsData,
  (state: OrganisationDetails) => state.heatPumps
);

export const selectLegalCorrespondenceAddress = createSelector(
  selectOrganisationDetailsData,
  (state: OrganisationDetails) => {
    return state.legalCorrespondenceAddress
      ? ({
          ...state.legalCorrespondenceAddress,
          isUsedAsLegalCorrespondence:
            state.address?.isUsedAsLegalCorrespondence,
        } as Address)
      : ({
          isUsedAsLegalCorrespondence:
            state.address?.isUsedAsLegalCorrespondence,
        } as Address);
  }
);

export const selectOrganisationStructure = createSelector(
  selectOrganisationDetailsData,
  (state: OrganisationDetails) => state.organisationStructure
);

export const selectResponsibleOfficer = createSelector(
  selectOrganisationDetailsData,
  (state: OrganisationDetails) => {
    return state.responsibleOfficer
      ? ({
          ...state.responsibleOfficer,
          isResponsibleOfficer: state.applicant?.isResponsibleOfficer,
        } as UserDetails)
      : null;
  }
);

export const selectResponsibleUndertaking = createSelector(
  selectOrganisationDetailsData,
  (state: OrganisationDetails) => state.responsibleUndertaking
);

export const selectIsFossilFuelBoilerSeller = createSelector(
  selectOrganisationDetailsData,
  (state: OrganisationDetails) => state.isFossilFuelBoilerSeller
);

export const selectUserDetails = createSelector(
  selectOrganisationDetailsData,
  (state: OrganisationDetails) => state.applicant
);

export const selectForSubmitOnboardingDetails = createSelector(
  selectOrganisationState,
  (state: OrganisationState) => {
    return toOnboardManufacturerCommand(state.organisationDetails.data!);
  }
);

export const selectAccountApprovalFiles = createSelector(
  selectOrganisationState,
  (state: OrganisationState) => state.accountApprovalFiles
);

export const selectAccountApprovalLoading = createSelector(
  selectAccountApprovalFiles,
  (files: MultiFileUploadState) => {
    return (
      files.uploadingFiles ||
      files.deletingFile ||
      files.retrievingFiles ||
      files.loading
    );
  }
);

export const selectAccountRejectionFiles = createSelector(
  selectOrganisationState,
  (state: OrganisationState) => state.accountRejectionFiles
);

export const selectAccountRejectionLoading = createSelector(
  selectAccountRejectionFiles,
  (files: MultiFileUploadState) => {
    return (
      files.uploadingFiles ||
      files.deletingFile ||
      files.retrievingFiles ||
      files.loading
    );
  }
);

export const selectForApproveOrganisationDto = createSelector(
  selectOrganisationState,
  (state: OrganisationState) => {
    return toApproveManufacturerOrganisationCommand(
      state.organisationDetails.data!,
      state.accountApproval!
    );
  }
);

export const selectForRejectOrganisationDto = createSelector(
  selectOrganisationState,
  (state: OrganisationState) => {
    return toRejectManufacturerOrganisationCommand(
      state.organisationDetails.data!,
      state.accountRejection!
    );
  }
);

export const selectForEditOrganisationDto = createSelector(
  selectOrganisationState,
  (state: OrganisationState) => {
    return toEditManufacturerOrganisationCommand(
      state.organisationDetails.data!,
      state.accountApproval!
    );
  }
);

export const selectIsSchemePartipantFormValue = createSelector(
  selectOrganisationState,
  (state: OrganisationState) => state.isSchemePartipantFormValue
);

export const selectIsNonSchemePartipant = createSelector(
  selectOrganisationState,
  (state: OrganisationState) =>
    state.isSchemePartipantFormValue === 'Yes' ? false : true
);

export const selectChangeIsNonSchemeParticipantLoading = createSelector(
  selectOrganisationState,
  (state: OrganisationState) => state.changeIsNonSchemeParticipant.loading
);
