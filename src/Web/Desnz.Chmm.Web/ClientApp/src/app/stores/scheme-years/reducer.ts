import { createReducer, on } from '@ngrx/store';
import { HttpState, defaultHttpState } from '../http-state';
import { SchemeYearActions } from '..';
import { SchemeYearDto } from '../organisation-summary/dtos/scheme-year.dto';
import { SchemeYearsState } from './state';

const defaultState: SchemeYearsState = {
  schemeYears: defaultHttpState<SchemeYearDto[]>(),
  selectedSchemeYear: null,
};

export const schemeYearsReducer = createReducer(
  defaultState,
  on(SchemeYearActions.getSchemeYears, (state, _): SchemeYearsState => {
    return {
      ...state,
      schemeYears: {
        ...state.schemeYears,
        loading: true,
        errorMessage: null,
      },
    };
  }),
  on(SchemeYearActions.onGetSchemeYears, (state, action): SchemeYearsState => {
    return {
      ...state,
      schemeYears: {
        ...state.schemeYears,
        loading: false,
        data: [...action.schemeYears],
      },
    };
  }),
  on(
    SchemeYearActions.onGetSchemeYearsError,
    (state, action): SchemeYearsState => {
      return {
        ...state,
        schemeYears: {
          ...state.schemeYears,
          loading: false,
          errorMessage: action.message,
        },
      };
    }
  ),
  on(
    SchemeYearActions.goToSchemeYear,
    SchemeYearActions.setSelectedSchemeYear,
    (state, action): SchemeYearsState => {
      return {
        ...state,
        selectedSchemeYear: { ...action.schemeYear },
      };
    }
  )
);
