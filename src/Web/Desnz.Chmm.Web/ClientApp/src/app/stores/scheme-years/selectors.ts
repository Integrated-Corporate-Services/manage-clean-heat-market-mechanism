import { AppState } from '..';

export const selectSchemeYearsState = (state: AppState) =>
  state.schemeYearsState.schemeYears;

export const selectSelectedSchemeYear = (state: AppState) =>
  state.schemeYearsState.selectedSchemeYear;
