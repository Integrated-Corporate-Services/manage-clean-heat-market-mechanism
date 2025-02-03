import { createReducer, on } from '@ngrx/store';
import { HttpState } from '../http-state';
import { ContactsActions } from '..';
import { ContactOrganisationDto } from '../onboarding/dtos/contact-organisation-dto';

const defaultState: HttpState<ContactOrganisationDto[]> = {
  loading: false,
  errorMessage: null,
  data: null,
};

export const contactsReducer = createReducer(
  defaultState,
  on(
    ContactsActions.getContactableOrganisations,
    (state, _): HttpState<ContactOrganisationDto[]> => {
      return {
        ...state,
        loading: true,
        errorMessage: null,
      };
    }
  ),
  on(
    ContactsActions.onGetContactableOrganisations,
    (state, action): HttpState<ContactOrganisationDto[]> => {
      return {
        ...state,
        loading: false,
        data: [...action.organisations],
      };
    }
  ),
  on(
    ContactsActions.onGetContactableOrganisationsError,
    (state, action): HttpState<ContactOrganisationDto[]> => {
      return {
        ...state,
        loading: false,
        errorMessage: action.message,
      };
    }
  )
);
