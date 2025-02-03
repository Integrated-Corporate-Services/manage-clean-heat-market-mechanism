import { createSelector } from '@ngrx/store';
import { AppState } from '..';
import { SchemeYearConfigurationState } from './state';

export const selectSchemeYearConfigurationState = (state: AppState) =>
  state.schemeYearConfigurationState;

export const selectSchemeYears = createSelector(
  selectSchemeYearConfigurationState,
  (state: SchemeYearConfigurationState) => state.schemeYears
);

export const selectSchemeYear = createSelector(
  selectSchemeYearConfigurationState,
  (state: SchemeYearConfigurationState) => state.schemeYear
);

export const selectSchemeYearConfiguration = createSelector(
  selectSchemeYearConfigurationState,
  (state: SchemeYearConfigurationState) => state.schemeYearConfiguration
);

export const selectUpdateSchemeYearConfigurationCommand = createSelector(
  selectSchemeYearConfigurationState,
  (state: SchemeYearConfigurationState) => state.updateSchemeYearConfigurationCommand
);
