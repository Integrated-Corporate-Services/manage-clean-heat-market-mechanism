import { createReducer, on } from '@ngrx/store';
import { OrganisationState } from './state';
import * as OnboardingActions from './actions';
import { toOrganisationDetails } from './utils';
import { Address } from 'src/app/manufacturer/onboarding/models/address';
import { MultiFileUploadState } from 'src/app/shared/store/MultiFileUploadState';

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

const defaultState: OrganisationState = {
  organisationDetails: {
    ...defaultHttpState,
    data: {
      id: null,
      organisationStructure: null,
      responsibleUndertaking: null,
      address: null,
      legalCorrespondenceAddress: null,
      isFossilFuelBoilerSeller: null,
      heatPumps: null,
      applicant: null,
      responsibleOfficer: null,
      creditContactDetails: null,
      isNonSchemeParticipant: null,
    },
  },
  submitOnboarding: defaultHttpState,
  accountApprovalFiles: defaultMultiFileUploadState,
  accountRejectionFiles: defaultMultiFileUploadState,
  accountApproval: null,
  accountRejection: null,
  changeIsNonSchemeParticipant: defaultHttpState,
  isSchemePartipantFormValue: null,
};

export const onboardingReducer = createReducer(
  defaultState,
  on(
    OnboardingActions.storeIsOnBehalfOfGroup,
    (state, action): OrganisationState => {
      return {
        ...state,
        organisationDetails: {
          ...state.organisationDetails,
          data: {
            ...state.organisationDetails.data!,
            organisationStructure: { ...action.organisationStructure },
          },
        },
      };
    }
  ),
  on(
    OnboardingActions.storeResponsibleUndertaking,
    (state, action): OrganisationState => {
      return {
        ...state,
        organisationDetails: {
          ...state.organisationDetails,
          data: {
            ...state.organisationDetails.data!,
            responsibleUndertaking: action.responsibleUndertaking,
          },
        },
      };
    }
  ),
  on(OnboardingActions.storeAddress, (state, action): OrganisationState => {
    let isUsedAsLegalCorrespondence =
      state.organisationDetails.data!.address?.isUsedAsLegalCorrespondence;
    let address = {
      ...action.address,
      isUsedAsLegalCorrespondence: isUsedAsLegalCorrespondence,
    };
    let legalCorrespondenceAddress = null;
    if (state.organisationDetails.data!.legalCorrespondenceAddress) {
      legalCorrespondenceAddress =
        isUsedAsLegalCorrespondence === 'Yes'
          ? {
              ...action.address,
              id: state.organisationDetails.data?.legalCorrespondenceAddress.id,
            }
          : {
              ...state.organisationDetails.data!.legalCorrespondenceAddress,
              id: state.organisationDetails.data?.legalCorrespondenceAddress.id,
            };
    }
    return {
      ...state,
      organisationDetails: {
        ...state.organisationDetails,
        data: {
          ...state.organisationDetails.data!,
          address: address,
          legalCorrespondenceAddress: legalCorrespondenceAddress,
        },
      },
    };
  }),
  on(
    OnboardingActions.storeLegalCorrespondenceAddress,
    (state, action): OrganisationState => {
      let legalCorrespondenceAddress: Address | null = null;
      if (action.address.isUsedAsLegalCorrespondence === 'Yes') {
        legalCorrespondenceAddress = {
          ...state.organisationDetails.data!.address!,
          id: state.organisationDetails.data?.legalCorrespondenceAddress?.id,
        };
      } else if (action.address.isUsedAsLegalCorrespondence === 'No') {
        legalCorrespondenceAddress = {
          id: state.organisationDetails.data?.legalCorrespondenceAddress?.id,
          lineOne: action.address.lineOne!,
          lineTwo: action.address.lineTwo!,
          city: action.address.city!,
          county: action.address.county!,
          postcode: action.address.postcode!,
        };
      }
      return {
        ...state,
        organisationDetails: {
          ...state.organisationDetails,
          data: {
            ...state.organisationDetails.data!,
            address: {
              ...state.organisationDetails.data!.address!,
              isUsedAsLegalCorrespondence:
                action.address.isUsedAsLegalCorrespondence,
            },
            legalCorrespondenceAddress: legalCorrespondenceAddress,
            isNonSchemeParticipant: false,
          },
        },
      };
    }
  ),
  on(
    OnboardingActions.storeIsNonSchemeParticipant,
    (state, action): OrganisationState => {
      return {
        ...state,
        organisationDetails: {
          ...state.organisationDetails,
          data: {
            ...state.organisationDetails.data!,
            isNonSchemeParticipant: true,
            legalCorrespondenceAddress: null,
            address: {
              ...state.organisationDetails.data!.address!,
              isUsedAsLegalCorrespondence: 'IsNonSchemeParticipant',
            },
          },
        },
      };
    }
  ),
  on(
    OnboardingActions.storeIsFossilFuelBoilerSeller,
    (state, action): OrganisationState => {
      return {
        ...state,
        organisationDetails: {
          ...state.organisationDetails,
          data: {
            ...state.organisationDetails.data!,
            isFossilFuelBoilerSeller: action.isFossilFuelBoilerSeller,
          },
        },
      };
    }
  ),
  on(
    OnboardingActions.storeHeatPumpBrands,
    (state, action): OrganisationState => {
      return {
        ...state,
        organisationDetails: {
          ...state.organisationDetails,
          data: {
            ...state.organisationDetails.data!,
            heatPumps: { ...action.heatPumps },
          },
        },
      };
    }
  ),
  on(OnboardingActions.storeUserDetails, (state, action): OrganisationState => {
    let isResponsibleOfficer =
      state.organisationDetails.data!.applicant?.isResponsibleOfficer;
    let userDetails = {
      ...action.userDetails,
      isResponsibleOfficer: isResponsibleOfficer,
    };
    let responsibleOfficer = null;
    if (state.organisationDetails.data!.responsibleOfficer) {
      responsibleOfficer =
        isResponsibleOfficer === 'Yes'
          ? {
              ...action.userDetails,
              id: state.organisationDetails.data?.responsibleOfficer.id,
            }
          : {
              ...state.organisationDetails.data!.responsibleOfficer,
              id: state.organisationDetails.data?.responsibleOfficer.id,
            };
    }
    return {
      ...state,
      organisationDetails: {
        ...state.organisationDetails,
        data: {
          ...state.organisationDetails.data!,
          applicant: userDetails,
          responsibleOfficer: responsibleOfficer,
        },
      },
    };
  }),
  on(
    OnboardingActions.storeResponsibleOfficer,
    (state, action): OrganisationState => {
      return {
        ...state,
        organisationDetails: {
          ...state.organisationDetails,
          data: {
            ...state.organisationDetails.data!,
            applicant: {
              ...state.organisationDetails.data!.applicant!,
              isResponsibleOfficer: action.userDetails.isResponsibleOfficer,
            },
            responsibleOfficer:
              action.userDetails.isResponsibleOfficer === 'Yes'
                ? {
                    ...state.organisationDetails.data!.applicant!,
                    id: state.organisationDetails.data?.responsibleOfficer?.id,
                  }
                : {
                    id: state.organisationDetails.data?.responsibleOfficer?.id,
                    fullName: action.userDetails.fullName!,
                    emailAddress: action.userDetails.emailAddress!,
                    jobTitle: action.userDetails.jobTitle!,
                    organisation: action.userDetails.organisation!,
                    telephoneNumber: action.userDetails.telephoneNumber!,
                  },
          },
        },
      };
    }
  ),
  on(
    OnboardingActions.storeContactDetailsForCt,
    (state, action): OrganisationState => {
      return {
        ...state,
        organisationDetails: {
          ...state.organisationDetails,
          data: {
            ...state.organisationDetails.data!,
            creditContactDetails: action.contactDetails
              ? { ...action.contactDetails }
              : null,
          },
        },
      };
    }
  ),
  on(
    OnboardingActions.getOrganisationDetails,
    (state, _): OrganisationState => {
      return {
        ...state,
        organisationDetails: { ...defaultHttpState, loading: true },
      };
    }
  ),
  on(
    OnboardingActions.onGetOrganisationDetails,
    (state, action): OrganisationState => {
      return {
        ...state,
        organisationDetails: {
          ...state.submitOnboarding,
          loading: false,
          data: toOrganisationDetails(action.organisationDetails),
        },
      };
    }
  ),
  on(
    OnboardingActions.getAccountApprovalFiles,
    (state): OrganisationState => {
      return {
        ...state,
        accountApprovalFiles: {
          ...state.accountApprovalFiles,
          retrievingFiles: true,
          error: null,
        },
      };
    }
  ),
  on(
    OnboardingActions.onGetAccountApprovalFiles,
    (state, action): OrganisationState => {
      return {
        ...state,
        accountApprovalFiles: {
          ...state.accountApprovalFiles,
          retrievingFiles: false,
          uploadingFiles: false,
          deletingFile: false,
          fileNames: action.files,
        },
      };
    }
  ),
  on(
    OnboardingActions.uploadAccountApprovalFile,
    (state): OrganisationState => {
      return {
        ...state,
        accountApprovalFiles: {
          ...state.accountApprovalFiles,
          uploadingFiles: true,
          error: null,
        },
      };
    }
  ),
  on(
    OnboardingActions.deleteAccountApprovalFile,
    (state): OrganisationState => {
      return {
        ...state,
        accountApprovalFiles: {
          ...state.accountApprovalFiles,
          deletingFile: true,
          error: null,
        },
      };
    }
  ),
  on(
    OnboardingActions.onErrorUploadAccountApprovalFile,
    (state, action): OrganisationState => {
      return {
        ...state,
        accountApprovalFiles: {
          ...state.accountApprovalFiles,
          retrievingFiles: false,
          uploadingFiles: false,
          error: action.error,
        },
      };
    }
  ),
  on(
    OnboardingActions.onErrorDeleteAccountApprovalFile,
    (state, action): OrganisationState => {
      return {
        ...state,
        accountApprovalFiles: {
          ...state.accountApprovalFiles,
          retrievingFiles: false,
          uploadingFiles: false,
          error: action.error,
        },
      };
    }
  ),
  on(
    OnboardingActions.storeAccountApproval,
    (state, action): OrganisationState => {
      return {
        ...state,
        accountApproval: { ...action.accountApproval },
      };
    }
  ),
  on(
    OnboardingActions.getAccountRejectionFiles,
    (state): OrganisationState => {
      return {
        ...state,
        accountRejectionFiles: {
          ...state.accountRejectionFiles,
          retrievingFiles: true,
          error: null,
        },
      };
    }
  ),
  on(
    OnboardingActions.onGetAccountRejectionFiles,
    (state, action): OrganisationState => {
      return {
        ...state,
        accountRejectionFiles: {
          ...state.accountRejectionFiles,
          retrievingFiles: false,
          uploadingFiles: false,
          deletingFile: false,
          fileNames: action.files,
        },
      };
    }
  ),
  on(
    OnboardingActions.uploadAccountRejectionFile,
    (state): OrganisationState => {
      return {
        ...state,
        accountRejectionFiles: {
          ...state.accountRejectionFiles,
          uploadingFiles: true,
          error: null,
        },
      };
    }
  ),
  on(
    OnboardingActions.deleteAccountRejectionFile,
    (state): OrganisationState => {
      return {
        ...state,
        accountRejectionFiles: {
          ...state.accountRejectionFiles,
          deletingFile: true,
          error: null,
        },
      };
    }
  ),
  on(
    OnboardingActions.onErrorUploadAccountRejectionFile,
    (state, action): OrganisationState => {
      return {
        ...state,
        accountRejectionFiles: {
          ...state.accountRejectionFiles,
          retrievingFiles: false,
          uploadingFiles: false,
          error: action.error,
        },
      };
    }
  ),
  on(
    OnboardingActions.onErrorDeleteAccountRejectionFile,
    (state, action): OrganisationState => {
      return {
        ...state,
        accountRejectionFiles: {
          ...state.accountRejectionFiles,
          retrievingFiles: false,
          uploadingFiles: false,
          error: action.error,
        },
      };
    }
  ),
  on(
    OnboardingActions.storeAccountRejection,
    (state, action): OrganisationState => {
      return {
        ...state,
        accountRejection: { ...action.accountRejection },
      };
    }
  ),
  on(
    OnboardingActions.submitOnboardingDetails,
    OnboardingActions.approveAccount,
    OnboardingActions.editAccount,
    (state, _): OrganisationState => {
      return {
        ...state,
        organisationDetails: {
          ...state.organisationDetails,
          loading: true,
          errorMessage: null,
        },
      };
    }
  ),
  on(
    OnboardingActions.onSubmitOnboardingDetails,
    OnboardingActions.onApproveAccount,
    OnboardingActions.onEditAccount,
    (state, _): OrganisationState => {
      return {
        ...state,
        organisationDetails: { ...state.organisationDetails, loading: false },
      };
    }
  ),
  on(OnboardingActions.onError, (state, action): OrganisationState => {
    return {
      ...state,
      organisationDetails: {
        ...state.organisationDetails,
        errorMessage: action.message,
        loading: false,
      },
    };
  }),
  on(
    OnboardingActions.clearIsSchemePartipantFormValue,
    (state, _): OrganisationState => {
      return {
        ...state,
        isSchemePartipantFormValue: null,
      };
    }
  ),
  on(
    OnboardingActions.storeIsSchemePartipantFormValue,
    (state, action): OrganisationState => {
      return {
        ...state,
        isSchemePartipantFormValue: action.isSchemeParticipant,
      };
    }
  ),
  on(
    OnboardingActions.changeIsNonSchemeParticipant,
    (state, _): OrganisationState => {
      return {
        ...state,
        changeIsNonSchemeParticipant: {
          ...state.changeIsNonSchemeParticipant,
          loading: true,
          errorMessage: null,
        },
      };
    }
  ),
  on(
    OnboardingActions.onChangeIsNonSchemeParticipant,
    (state, _): OrganisationState => {
      return {
        ...state,
        changeIsNonSchemeParticipant: {
          ...state.changeIsNonSchemeParticipant,
          loading: false,
        },
      };
    }
  ),
  on(
    OnboardingActions.onChangeIsNonSchemeParticipantError,
    (state, action): OrganisationState => {
      return {
        ...state,
        changeIsNonSchemeParticipant: {
          ...state.changeIsNonSchemeParticipant,
          loading: false,
          errorMessage: action.message,
        },
      };
    }
  )
);
