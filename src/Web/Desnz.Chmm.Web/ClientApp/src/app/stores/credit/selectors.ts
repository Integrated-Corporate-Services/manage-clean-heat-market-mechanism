import { createSelector } from '@ngrx/store';
import { AppState } from '..';
import { CreditState, EditCreditState } from './state';

export const selectCreditState = (state: AppState) => state.creditState;

export const selectCreditBalance = createSelector(
  selectCreditState,
  (state: CreditState) => state.creditBalance
);

export const selectCreditTransfer = createSelector(
  selectCreditState,
  (state: CreditState) => state.creditTransfer
);

export const selectIsLoading = createSelector(
  selectCreditState,
  (state: CreditState) => state.loading
);

export const selectEditCreditState = createSelector(
  selectCreditState,
  (state: CreditState) => state.editCredit
);

export const selectEditCreditAmendment = createSelector(
  selectEditCreditState,
  (state: EditCreditState) => state.creditAmendment
);

export const selectOrganisationId = createSelector(
  selectCreditState,
  (state: CreditState) => state.organisationId
);

export const selectCreditTransferHistory = createSelector(
  selectCreditState,
  (state: CreditState) => state.transferHistory
);
