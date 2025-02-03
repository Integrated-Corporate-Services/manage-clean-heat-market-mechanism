import { createReducer, on } from '@ngrx/store';
import { McsState } from './state';
import { McsActions } from '..';

const defaultState: McsState = {
  installationRequestSummaries: [],
  loading: false,
  errorMessage: null,
};

export const mcsReducer = createReducer(
  defaultState,
  on(McsActions.getMcsDownloads, (state, _): McsState => {
    return {
      ...state,
      loading: true,
      errorMessage: null,
    };
  }),
  on(McsActions.onGetMcsDownloads, (state, action): McsState => {
    return {
      ...state,
      loading: false,
      installationRequestSummaries: [...action.downloads],
    };
  }),
  on(McsActions.onError, (state, action): McsState => {
    return {
      ...state,
      loading: false,
      errorMessage: action.message,
    };
  })
);
