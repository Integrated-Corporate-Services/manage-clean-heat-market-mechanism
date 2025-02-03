import { createReducer, on } from '@ngrx/store';
import { CreditActions } from '..';
import { CreditState } from './state';

const defaultState: CreditState = {
  organisationId: null,
  editCredit: {
    creditAmendment: null,
    errorMessage: null,
    loading: false,
  },
  creditTransfer: null,
  creditBalance: null,
  isTransferWindowOpened: null,
  transferHistory: null,
  errorMessage: null,
  loading: false,
};

export const creditReducer = createReducer(
  defaultState,
  on(CreditActions.getCreditTransferHistory, (state, action): CreditState => {
    return {
      ...state,
      transferHistory: null,
    };
  }),
  on(CreditActions.onGetCreditTransferHistory, (state, action): CreditState => {
    return {
      ...state,
      transferHistory: action.transferHistory,
    };
  }),
  on(CreditActions.clearCreditTransfer, (state, action): CreditState => {
    return {
      ...state,
      creditTransfer: null,
    };
  }),
  on(CreditActions.storeCreditTransfer, (state, action): CreditState => {
    return {
      ...state,
      organisationId: action.organisationId,
      creditTransfer: { ...action.creditTransfer },
    };
  }),
  on(CreditActions.onSubmitCreditTransfer, (state, action): CreditState => {
    return {
      ...state,
      loading: false,
      errorMessage: null,
    };
  }),
  on(CreditActions.onGetCreditBalance, (state, action): CreditState => {
    return {
      ...state,
      loading: false,
      creditBalance: action.creditBalance,
    };
  }),
  on(CreditActions.onError, (state, action): CreditState => {
    return {
      ...state,
      loading: false,
      errorMessage: action.message,
    };
  }),
  on(CreditActions.storeCreditAmendment, (state, action): CreditState => {
    return {
      ...state,
      organisationId: action.organisationId,
      editCredit: {
        ...state.editCredit,
        creditAmendment: { ...action.creditAmendment },
      },
    };
  }),
  on(CreditActions.submitCreditAmendment, (state, _): CreditState => {
    return {
      ...state,
      editCredit: {
        ...state.editCredit,
        loading: true,
        errorMessage: null,
      },
    };
  }),
  on(CreditActions.onSubmitCreditAmendment, (state, _): CreditState => {
    return {
      ...state,
      editCredit: {
        ...state.editCredit,
        loading: false,
      },
    };
  }),
  on(CreditActions.clearCreditAmendment, (state, _): CreditState => {
    return {
      ...state,
      editCredit: {
        ...state.editCredit,
        creditAmendment: null,
        loading: false,
        errorMessage: null,
      },
    };
  }),
  on(CreditActions.onError, (state, action): CreditState => {
    return {
      ...state,
      loading: false,
      errorMessage: action.message,
    };
  }),
  on(
    CreditActions.onSubmitCreditAmendmentError,
    (state, action): CreditState => {
      return {
        ...state,
        editCredit: {
          ...state.editCredit,
          loading: false,
          errorMessage: action.message,
        },
      };
    }
  )
);
