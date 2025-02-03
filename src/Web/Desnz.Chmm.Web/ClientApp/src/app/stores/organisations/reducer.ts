import { createReducer, on } from '@ngrx/store';
import { Organisation } from 'src/app/navigation/models/organisation';
import { HttpState } from '../http-state';
import { OrganisationsActions } from '..';

const defaultState: HttpState<Organisation[]> = {
  loading: false,
  errorMessage: null,
  data: null,
};

export const availableForTransferReducer = createReducer(
  defaultState,
  on(
    OrganisationsActions.getOrganisationsAvailableForTransfer,
    (state, _): HttpState<Organisation[]> => {
      return {
        ...state,
        loading: true,
        errorMessage: null,
      };
    }
  ),
  on(
    OrganisationsActions.onGetOrganisationsAvailableForTransfer,
    (state, action): HttpState<Organisation[]> => {
      return {
        ...state,
        loading: false,
        data: [...action.organisations],
      };
    }
  ),
  on(
    OrganisationsActions.onGetOrganisationsAvailableForTransferError,
    (state, action): HttpState<Organisation[]> => {
      return {
        ...state,
        loading: false,
        errorMessage: action.message,
      };
    }
  )
);
