import * as AdministratorRejectionActions from './actions';
import { createReducer, on } from '@ngrx/store';
import { HttpState } from '../http-state';
import { OrganisationRejectionCommentsDto } from './dtos/organisation-rejection-comments.dto';

const defaultState: HttpState<OrganisationRejectionCommentsDto> = {
  loading: false,
  errorMessage: null,
  data: null,
};

export const administratorRejectionReducer = createReducer(
  defaultState,
  on(AdministratorRejectionActions.getRejectionComments, (state, _) => {
    return {
      ...state,
      loading: true,
      errorMessage: null,
    };
  }),
  on(AdministratorRejectionActions.onGetRejectionComments, (state, action) => {
    return {
      ...state,
      loading: false,
      data: { ...action.organisationRejectionCommentsDto },
    };
  }),
  on(
    AdministratorRejectionActions.onGetRejectionCommentsError,
    (state, action) => {
      return {
        ...state,
        loading: false,
        errorMessage: action.error,
      };
    }
  )
);
