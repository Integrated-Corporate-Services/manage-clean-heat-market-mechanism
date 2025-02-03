import { createReducer, on } from '@ngrx/store';
import { LicenceHolderState } from './state';
import * as LicenceHolderActions from './actions';
import * as moment from 'moment';

const defaultHttpState = {
  loading: false,
  errorMessage: null,
  data: null,
};

const defaultState: LicenceHolderState = {
  currentSchemeYear: defaultHttpState,
  firstSchemeYear: defaultHttpState,
  linked: defaultHttpState,
  unlinked: defaultHttpState,
  editLinkedLicenceHolder: defaultHttpState,
  editLinkedLicenceHolderFormValue: null,
  selectedLicenceHolder: null,
  selectedLicenceHolderLink: null,
  linkLicenceHolderFormValue: null,
  startOfLink: null,
  endOfLink: null,
};

export const licenceHolderReducer = createReducer(
  defaultState,
  on(
    LicenceHolderActions.linkLicenceHolder,
    (state, action): LicenceHolderState => {
      return {
        ...state,
        selectedLicenceHolder:
          state.unlinked?.data?.find((x) => x.id === action.licenceHolderId) ||
          null,
      };
    }
  ),
  on(
    LicenceHolderActions.getLinkedLicenceHolders,
    (state, _): LicenceHolderState => {
      return {
        ...state,
        linked: {
          ...state.linked,
          loading: true,
          errorMessage: null,
        },
      };
    }
  ),
  on(
    LicenceHolderActions.getUnlinkedLicenceHolders,
    (state, _): LicenceHolderState => {
      return {
        ...state,
        unlinked: {
          ...state.unlinked,
          loading: true,
          errorMessage: null,
        },
      };
    }
  ),
  on(
    LicenceHolderActions.onGetLinkedLicenceHolders,
    (state, action): LicenceHolderState => {
      return {
        ...state,
        linked: {
          ...state.linked,
          loading: false,
          data: action.licenceHolderLinks.map((l) => {
            return {
              id: l.id,
              licenceHolderId: l.licenceHolderId,
              licenceHolderName: l.licenceHolderName,
              organisationName: l.organisationName,
              organisationId: l.organisationId,
              startDate:
                l.startDate == action.schemeYearStartDate
                  ? 'Start of scheme'
                  : moment(l.startDate, 'YYYY-MM-DD').format('DD MMMM YYYY'),
              endDate:
                l.endDate !== null
                  ? moment(l.endDate, 'YYYY-MM-DD').format('DD MMMM YYYY')
                  : 'Ongoing',
            };
          }),
        },
      };
    }
  ),
  on(
    LicenceHolderActions.onGetUnlinkedLicenceHolders,
    (state, action): LicenceHolderState => {
      return {
        ...state,
        unlinked: {
          ...state.unlinked,
          loading: false,
          data: action.licenceHolders,
        },
      };
    }
  ),
  on(
    LicenceHolderActions.onErrorGetLinkedLicenceHolders,
    (state, action): LicenceHolderState => {
      return {
        ...state,
        linked: {
          ...state.linked,
          loading: false,
          errorMessage: action.message,
        },
      };
    }
  ),
  on(
    LicenceHolderActions.onErrorGetUnlinkedLicenceHolders,
    (state, action): LicenceHolderState => {
      return {
        ...state,
        unlinked: {
          ...state.unlinked,
          loading: false,
          errorMessage: action.message,
        },
      };
    }
  ),
  on(
    LicenceHolderActions.storeLinkLicenceHolderFormValue,
    (state, action): LicenceHolderState => {
      return {
        ...state,
        linkLicenceHolderFormValue: { ...action.linkLicenceHolderFormValue },
        startOfLink:
          action.linkLicenceHolderFormValue.linkStart === 'startOfScheme'
            ? 'Start of the scheme'
            : moment([
                action.linkLicenceHolderFormValue.year!,
                Number(action.linkLicenceHolderFormValue.month) - 1,
                action.linkLicenceHolderFormValue.day!,
              ]).format('DD/MM/YYYY'),
      };
    }
  ),
  on(
    LicenceHolderActions.getFirstSchemeYear,
    (state, _): LicenceHolderState => {
      return {
        ...state,
        firstSchemeYear: {
          ...state.firstSchemeYear,
          loading: true,
          errorMessage: null,
        },
      };
    }
  ),
  on(
    LicenceHolderActions.onGetFirstSchemeYear,
    (state, action): LicenceHolderState => {
      return {
        ...state,
        firstSchemeYear: {
          ...state.firstSchemeYear,
          loading: false,
          data: { ...action.schemeYear },
        },
      };
    }
  ),
  on(
    LicenceHolderActions.onGetFirstSchemeYearError,
    (state, action): LicenceHolderState => {
      return {
        ...state,
        firstSchemeYear: {
          ...state.firstSchemeYear,
          loading: false,
          errorMessage: action.message,
        },
      };
    }
  ),
  on(
    LicenceHolderActions.getCurrentSchemeYear,
    (state, _): LicenceHolderState => {
      return {
        ...state,
        currentSchemeYear: {
          ...state.currentSchemeYear,
          loading: true,
          errorMessage: null,
        },
      };
    }
  ),
  on(
    LicenceHolderActions.onGetCurrentSchemeYear,
    (state, action): LicenceHolderState => {
      return {
        ...state,
        currentSchemeYear: {
          ...state.currentSchemeYear,
          loading: false,
          data: { ...action.schemeYear },
        },
      };
    }
  ),
  on(
    LicenceHolderActions.onGetCurrentSchemeYearError,
    (state, action): LicenceHolderState => {
      return {
        ...state,
        currentSchemeYear: {
          ...state.currentSchemeYear,
          loading: false,
          errorMessage: action.message,
        },
      };
    }
  ),
  on(
    LicenceHolderActions.clearLinkLicenceHolderFormValue,
    (state, _): LicenceHolderState => {
      return {
        ...state,
        linkLicenceHolderFormValue: null,
      };
    }
  ),
  on(
    LicenceHolderActions.selectLinkedLincenceHolder,
    (state, action): LicenceHolderState => {
      return {
        ...state,
        selectedLicenceHolderLink: { ...action.licenceHolderLink },
        editLinkedLicenceHolderFormValue: null,
      };
    }
  ),
  on(
    LicenceHolderActions.storeEditLinkedLicenceHolderFormValue,
    (state, action): LicenceHolderState => {
      return {
        ...state,
        editLinkedLicenceHolderFormValue: {
          ...action.editLinkedLicenceHolderFormValue,
        },
        endOfLink: moment([
          action.editLinkedLicenceHolderFormValue.year!,
          Number(action.editLinkedLicenceHolderFormValue.month) - 1,
          action.editLinkedLicenceHolderFormValue.day!,
        ]).format('DD/MM/YYYY'),
      };
    }
  ),
  on(
    LicenceHolderActions.editLinkedLicenceHolder,
    (state, _): LicenceHolderState => {
      return {
        ...state,
        editLinkedLicenceHolder: {
          ...state.editLinkedLicenceHolder,
          loading: true,
          errorMessage: null,
        },
      };
    }
  ),
  on(
    LicenceHolderActions.onEditLinkedLicenceHolder,
    (state, _): LicenceHolderState => {
      return {
        ...state,
        editLinkedLicenceHolder: {
          ...state.editLinkedLicenceHolder,
          loading: false,
        },
      };
    }
  ),
  on(
    LicenceHolderActions.onEditLinkedLicenceHolderError,
    (state, action): LicenceHolderState => {
      return {
        ...state,
        editLinkedLicenceHolder: {
          ...state.editLinkedLicenceHolder,
          loading: false,
          errorMessage: action.message,
        },
      };
    }
  )
);
