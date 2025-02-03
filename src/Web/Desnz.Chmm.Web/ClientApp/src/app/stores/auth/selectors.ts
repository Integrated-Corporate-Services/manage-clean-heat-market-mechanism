import { createSelector } from '@ngrx/store';
import { AppState } from '..';
import { AuthState } from './reducer';
import { isAdmin } from 'src/app/shared/auth-utils';

export const selectAuth = (state: AppState) => state.auth;

export const selectWhoAmI = createSelector(
  selectAuth,
  (state: AuthState) => state.whoAmI
);

export const selectIsAuthenticated = createSelector(
  selectAuth,
  (state: AuthState) => state.whoAmI !== null
);

export const selectIsAdmin = createSelector(selectAuth, (state: AuthState) =>
  isAdmin(state.whoAmI)
);

export const selectShowManufacturerServiceNavbarLinks = createSelector(
  selectAuth,
  (state: AuthState) => {
    let whoAmI = state.whoAmI;
    if (whoAmI !== null) {
      return (
        whoAmI.roles.includes('Manufacturer') &&
        whoAmI.status === 'Active' &&
        whoAmI.organisationId !== null
      );
    }
    return false;
  }
);

export const selectOrganisationId = createSelector(
  selectAuth,
  (state: AuthState) => state.whoAmI?.organisationId ?? null
);
