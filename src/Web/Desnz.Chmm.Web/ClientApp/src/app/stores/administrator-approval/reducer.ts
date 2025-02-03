import * as AdministratorApprovalActions from './actions';
import { createReducer, on } from '@ngrx/store';
import { HttpState } from '../http-state';
import { OrganisationApprovalCommentsDto } from './dtos/organisation-approval-comments.dto';

const defaultState: HttpState<OrganisationApprovalCommentsDto> = {
  loading: false,
  errorMessage: null,
  data: null,
};

export const administratorApprovalReducer = createReducer(
  defaultState,
  on(AdministratorApprovalActions.getApprovalComments, (state, _) => {
    return {
      ...state,
      loading: true,
      errorMessage: null,
    };
  }),
  on(AdministratorApprovalActions.onGetApprovalComments, (state, action) => {
    return {
      ...state,
      loading: false,
      data: { ...action.organisationApprovalCommentsDto },
    };
  }),
  on(
    AdministratorApprovalActions.onGetApprovalCommentsError,
    (state, action) => {
      return {
        ...state,
        loading: false,
        errorMessage: action.error,
      };
    }
  )
);
