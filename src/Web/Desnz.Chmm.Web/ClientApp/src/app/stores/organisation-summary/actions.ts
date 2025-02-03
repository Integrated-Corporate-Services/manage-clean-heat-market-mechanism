import { createAction, props } from '@ngrx/store';
import { SchemeYearSummaryConfigurationDto } from './dtos/scheme-year-summary-configuration.dto';
import { BoilerSalesSummaryDto } from './dtos/boiler-sales-summary.dto';
import { ObligationSummaryDto } from './dtos/obligation-summary.dto';
import { CreditLedgerSummaryDto } from './dtos/credit-ledger-summary.dto';

export const getSchemeYearConfigurationSummary = createAction(
  '[Organisation summary] Get scheme year configuration summary',
  props<{
    organisationId: string;
    schemeYearId: string;
  }>()
);

export const onGetSchemeYearConfigurationSummary = createAction(
  '[Organisation summary] On: Get scheme year configuration summary',
  props<{
    organisationId: string;
    schemeYearId: string;
    schemeYearParameters: SchemeYearSummaryConfigurationDto;
  }>()
);

export const onGetSchemeYearConfigurationSummaryError = createAction(
  '[Organisation summary] On Error: Get scheme year configuration summary',
  props<{ errorMessage: string }>()
);

export const getBoilerSalesSummary = createAction(
  '[Organisation summary] Get boiler sales summary',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onGetBoilerSalesSummary = createAction(
  '[Organisation summary] On: Get boiler sales summary',
  props<{ boilerSalesSummary: BoilerSalesSummaryDto }>()
);

export const onGetBoilerSalesSummaryError = createAction(
  '[Organisation summary] On Error: Get boiler sales summary',
  props<{ errorMessage: string }>()
);

export const getObligationSummary = createAction(
  '[Organisation summary] Get obligation summary',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onGetObligationSummary = createAction(
  '[Organisation summary] On: Get obligation summary',
  props<{ obligationSummary: ObligationSummaryDto }>()
);

export const onGetObligationSummaryError = createAction(
  '[Organisation summary] On Error: Get obligation summary',
  props<{ errorMessage: string }>()
);

export const getCreditLedgerSummary = createAction(
  '[Organisation summary] Get credit ledger summary',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onGetCreditLedgerSummary = createAction(
  '[Organisation summary] On: Get credit ledger summary',
  props<{ creditLedgerSummary: CreditLedgerSummaryDto }>()
);

export const onGetCreditLedgerSummaryError = createAction(
  '[Organisation summary] On Error: Get credit ledger summary',
  props<{ errorMessage: string }>()
);
