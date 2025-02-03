import { createReducer, on } from '@ngrx/store';
import { AuthActions } from '..';
import { WhoAmI } from './services/authentication.service';

export interface AuthState {
  whoAmI: WhoAmI | null;
}

export const initialState: AuthState = {
  whoAmI: null,
};

export const authReducer = createReducer(
  initialState,
  on(AuthActions.onGetWhoAmI, (state, action) => ({
    ...state,
    whoAmI: action.whoAmI !== null ? { ...action.whoAmI } : null,
  }))
);
