import { createReducer, on } from '@ngrx/store';
import { ObligationActions } from '..';
import { AmendObligationState as AmendObligationState } from './state';

const defaultState: AmendObligationState = {
  organisationId: null,
  editObligation: {
    obligationAmendment: null,
    loading: false,
    errorMessage: null,
  },
};

export const amendObligationReducer = createReducer(
  defaultState,
  on(
    ObligationActions.storeObligationAmendment,
    (state, action): AmendObligationState => {
      return {
        ...state,
        organisationId: action.organisationId,
        editObligation: {
          ...state.editObligation,
          obligationAmendment: { ...action.obligationAmendment },
          errorMessage: null,
        },
      };
    }
  ),
  on(
    ObligationActions.submitObligationAmendment,
    (state, _): AmendObligationState => {
      return {
        ...state,
        editObligation: {
          ...state.editObligation,
          loading: true,
          errorMessage: null,
        },
      };
    }
  ),
  on(
    ObligationActions.onSubmitObligationAmendment,
    (state, _): AmendObligationState => {
      return {
        ...state,
        editObligation: {
          ...state.editObligation,
          loading: false,
        },
      };
    }
  ),
  on(
    ObligationActions.clearObligationAmendment,
    (state, _): AmendObligationState => {
      return {
        ...state,
        editObligation: {
          ...state.editObligation,
          obligationAmendment: null,
          loading: false,
          errorMessage: null,
        },
      };
    }
  ),
  on(
    ObligationActions.onSubmitObligationAmendmentError,
    (state, action): AmendObligationState => {
      return {
        ...state,
        editObligation: {
          ...state.editObligation,
          loading: false,
          errorMessage: action.message,
        },
      };
    }
  )
);
