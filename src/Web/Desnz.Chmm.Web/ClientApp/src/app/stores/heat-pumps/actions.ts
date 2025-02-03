import { createAction, props } from '@ngrx/store';
import { PeriodCreditTotalsDto } from './dtos/period-credit-totals.dto';

export const getHeatPumpInstallations = createAction(
  '[Heat pumps] Get heat pumps',
  props<{
    schemeYearId: string;
    organisationId: string;
  }>()
);

export const onGetHeatPumpInstallations = createAction(
  '[Heat pumps] On: Get heat pumps',
  props<{
    heatPumpInstallations: PeriodCreditTotalsDto[];
  }>()
);

export const onGetHeatPumpInstallationsError = createAction(
  '[Heat pumps] On Error: Get heat pumps',
  props<{
    error: string;
  }>()
);
