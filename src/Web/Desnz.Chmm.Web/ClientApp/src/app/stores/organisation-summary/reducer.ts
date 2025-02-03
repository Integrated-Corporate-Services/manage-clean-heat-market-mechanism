import { OrganisationSummaryState } from './state';
import { createReducer, on } from '@ngrx/store';
import { OrganisationSummaryActions } from '..';
import { defaultHttpState } from '../http-state';
import { SchemeYearSummaryConfigurationDto } from './dtos/scheme-year-summary-configuration.dto';
import { BoilerSalesSummaryDto } from './dtos/boiler-sales-summary.dto';
import { ObligationSummaryDto } from './dtos/obligation-summary.dto';
import { CreditLedgerSummaryDto } from './dtos/credit-ledger-summary.dto';

const defaultState: OrganisationSummaryState = {
  schemeYearParameters: defaultHttpState<SchemeYearSummaryConfigurationDto>(),
  boilerSalesSummary: defaultHttpState<BoilerSalesSummaryDto>(),
  obligationSummary: defaultHttpState<ObligationSummaryDto>(),
  creditLedgerSummary: defaultHttpState<CreditLedgerSummaryDto>(),
};

export const organisationSummaryReducer = createReducer(
  defaultState,
  on(
    OrganisationSummaryActions.getSchemeYearConfigurationSummary,
    (state, _): OrganisationSummaryState => {
      return {
        ...state,
        boilerSalesSummary: defaultHttpState<BoilerSalesSummaryDto>(),
        obligationSummary: defaultHttpState<ObligationSummaryDto>(),
        creditLedgerSummary: defaultHttpState<CreditLedgerSummaryDto>(),
        schemeYearParameters: {
          ...defaultHttpState<SchemeYearSummaryConfigurationDto>(),
          loading: true
        }
      };
    }
  ),
  on(
    OrganisationSummaryActions.onGetSchemeYearConfigurationSummary,
    (state, action): OrganisationSummaryState => {
      return {
        ...state,
        schemeYearParameters: {
          ...state.schemeYearParameters,
          loading: false,
          data: { ...action.schemeYearParameters },
        },
      };
    }
  ),
  on(
    OrganisationSummaryActions.onGetSchemeYearConfigurationSummaryError,
    (state, action): OrganisationSummaryState => {
      return {
        ...state,
        schemeYearParameters: {
          ...state.schemeYearParameters,
          loading: false,
          errorMessage: action.errorMessage,
        },
      };
    }
  ),
  on(
    OrganisationSummaryActions.getBoilerSalesSummary,
    (state, _): OrganisationSummaryState => {
      return {
        ...state,
        boilerSalesSummary: {
          ...state.boilerSalesSummary,
          loading: true,
          errorMessage: null,
        },
      };
    }
  ),
  on(
    OrganisationSummaryActions.onGetBoilerSalesSummary,
    (state, action): OrganisationSummaryState => {
      return {
        ...state,
        boilerSalesSummary: {
          ...state.boilerSalesSummary,
          loading: false,
          data: { ...action.boilerSalesSummary },
        },
      };
    }
  ),
  on(
    OrganisationSummaryActions.onGetBoilerSalesSummaryError,
    (state, action): OrganisationSummaryState => {
      return {
        ...state,
        boilerSalesSummary: {
          ...state.boilerSalesSummary,
          loading: false,
          errorMessage: action.errorMessage,
        },
      };
    }
  ),
  on(
    OrganisationSummaryActions.getObligationSummary,
    (state, _): OrganisationSummaryState => {
      return {
        ...state,
        obligationSummary: {
          ...state.obligationSummary,
          loading: true,
          errorMessage: null,
        },
      };
    }
  ),
  on(
    OrganisationSummaryActions.onGetObligationSummary,
    (state, action): OrganisationSummaryState => {
      return {
        ...state,
        obligationSummary: {
          ...state.obligationSummary,
          loading: false,
          data: { ...action.obligationSummary },
        },
      };
    }
  ),
  on(
    OrganisationSummaryActions.onGetObligationSummaryError,
    (state, action): OrganisationSummaryState => {
      return {
        ...state,
        obligationSummary: {
          ...state.obligationSummary,
          loading: false,
          errorMessage: action.errorMessage,
        },
      };
    }
  ),
  on(
    OrganisationSummaryActions.getCreditLedgerSummary,
    (state, _): OrganisationSummaryState => {
      return {
        ...state,
        creditLedgerSummary: {
          ...state.creditLedgerSummary,
          loading: true,
          errorMessage: null,
        },
      };
    }
  ),
  on(
    OrganisationSummaryActions.onGetCreditLedgerSummary,
    (state, action): OrganisationSummaryState => {
      return {
        ...state,
        creditLedgerSummary: {
          ...state.creditLedgerSummary,
          loading: false,
          data: { ...action.creditLedgerSummary },
        },
      };
    }
  ),
  on(
    OrganisationSummaryActions.onGetCreditLedgerSummaryError,
    (state, action): OrganisationSummaryState => {
      return {
        ...state,
        creditLedgerSummary: {
          ...state.creditLedgerSummary,
          loading: false,
          errorMessage: action.errorMessage,
        },
      };
    }
  )
);
