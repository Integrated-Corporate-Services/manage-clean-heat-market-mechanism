import { createReducer, on } from '@ngrx/store';
import { defaultHttpState } from '../http-state';
import { SchemeYearConfigurationActions } from '..';
import { SchemeYearConfigurationState } from './state';
import { SchemeYearDto } from './dtos/scheme-year.dto';
import { SchemeYearConfigurationDto } from './dtos/scheme-year-configuration.dto';

const defaultState: SchemeYearConfigurationState = {
  schemeYears: defaultHttpState<SchemeYearDto[]>(),
  schemeYear: defaultHttpState<SchemeYearDto>(),
  schemeYearConfiguration: defaultHttpState<SchemeYearConfigurationDto>(),
  updateSchemeYearConfigurationCommand: null
};

export const schemeYearConfigurationReducer = createReducer(
  defaultState,
  on(SchemeYearConfigurationActions.getSchemeYears, (state, _): SchemeYearConfigurationState => {
    return {
      ...state,
      schemeYears: {
        loading: true,
        errorMessage: null,
        data: null,
      },
    };
  }),
  on(SchemeYearConfigurationActions.onGetSchemeYears, (state, action): SchemeYearConfigurationState => {
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
    SchemeYearConfigurationActions.onGetSchemeYearsError,
    (state, action): SchemeYearConfigurationState => {
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
  on(SchemeYearConfigurationActions.getSchemeYear, (state, _): SchemeYearConfigurationState => {
    return {
      ...state,
      schemeYear: {
        loading: true,
        errorMessage: null,
        data: null,
      },
      schemeYearConfiguration: {
        loading: true,
        errorMessage: null,
        data: null,
      }
    };
  }),
  on(SchemeYearConfigurationActions.onGetSchemeYear, (state, action): SchemeYearConfigurationState => {
    return {
      ...state,
      schemeYear: {
        ...state.schemeYear,
        loading: false,
        data: action.schemeYear,
      },
      schemeYearConfiguration: {
        ...state.schemeYearConfiguration,
        loading: false,
        data: action.schemeYearConfiguration,
      }
    };
  }),
  on(
    SchemeYearConfigurationActions.onGetSchemeYearError,
    (state, action): SchemeYearConfigurationState => {
      return {
        ...state,
        schemeYear: {
          ...state.schemeYear,
          loading: false,
          errorMessage: action.message,
        },
        schemeYearConfiguration: {
          ...state.schemeYearConfiguration,
          loading: false,
          errorMessage: action.message,
        },
      };
    }
  ),
  on(
    SchemeYearConfigurationActions.storeSchemeYearConfiguration,
    (state, action): SchemeYearConfigurationState => {
      return {
        ...state,
        updateSchemeYearConfigurationCommand: {
          ...state.updateSchemeYearConfigurationCommand,
          ...action.schemeYearConfiguration
        }
      };
    }
  ),
);
