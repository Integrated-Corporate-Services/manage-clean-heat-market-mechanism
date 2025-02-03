import * as HeatPumpInstallationsActions from './actions';
import { createReducer, on } from '@ngrx/store';
import { HttpState } from '../http-state';
import { PeriodCreditTotalsDto } from './dtos/period-credit-totals.dto';

const defaultState: HttpState<PeriodCreditTotalsDto[]> = {
  loading: false,
  errorMessage: null,
  data: null,
};

export const heatPumpInstallationsReducer = createReducer(
  defaultState,
  on(HeatPumpInstallationsActions.getHeatPumpInstallations, (state, _) => {
    return {
      ...state,
      loading: true,
      errorMessage: null,
    };
  }),
  on(HeatPumpInstallationsActions.onGetHeatPumpInstallations, (state, action) => {
    return {
      ...state,
      loading: false,
      data: [ ...action.heatPumpInstallations ],
    };
  }),
  on(
    HeatPumpInstallationsActions.onGetHeatPumpInstallationsError,
    (state, action) => {
      return {
        ...state,
        loading: false,
        errorMessage: action.error,
      };
    }
  )
);
