import { createSelector } from '@ngrx/store';
import { AppState } from '..';
import { OrganisationSummaryState } from './state';

export const selectOrganisationSummaryState = (state: AppState) =>
  state.organisationSummaryState;

export const selectSchemeYearParameters = createSelector(
  selectOrganisationSummaryState,
  (state: OrganisationSummaryState) => state.schemeYearParameters
);

export const selectBoilerSalesSummaryData = createSelector(
  selectOrganisationSummaryState,
  (state: OrganisationSummaryState) => state.boilerSalesSummary.data
);

export const selectObligationSummaryData = createSelector(
  selectOrganisationSummaryState,
  (state: OrganisationSummaryState) => state.obligationSummary.data
);

export const selectObligationCalculationLoading = createSelector(
  selectOrganisationSummaryState,
  (state: OrganisationSummaryState) =>
    state.obligationSummary.loading || state.boilerSalesSummary.loading
);

export const selectCreditLedgerSummary = createSelector(
  selectOrganisationSummaryState,
  (state: OrganisationSummaryState) => state.creditLedgerSummary
);
