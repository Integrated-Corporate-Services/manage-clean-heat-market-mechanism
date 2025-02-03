import { createSelector } from '@ngrx/store';
import { AppState } from '..';
import { EditObligationState, AmendObligationState } from './state';

export const selectObligationState = (state: AppState) => state.obligationState;

export const selectEditObligationState = createSelector(
  selectObligationState,
  (state: AmendObligationState) => state.editObligation
);

export const selectEditObligationAmendment = createSelector(
  selectEditObligationState,
  (state: EditObligationState) => state.obligationAmendment
);

export const selectOrganisationId = createSelector(
  selectObligationState,
  (state: AmendObligationState) => state.organisationId
);
