import { HttpState } from '../http-state';
import { BoilerSalesSummaryDto } from './dtos/boiler-sales-summary.dto';
import { CreditLedgerSummaryDto } from './dtos/credit-ledger-summary.dto';
import { ObligationSummaryDto } from './dtos/obligation-summary.dto';
import { SchemeYearSummaryConfigurationDto } from './dtos/scheme-year-summary-configuration.dto';

export interface OrganisationSummaryState {
  schemeYearParameters: HttpState<SchemeYearSummaryConfigurationDto>;
  boilerSalesSummary: HttpState<BoilerSalesSummaryDto>;
  obligationSummary: HttpState<ObligationSummaryDto>;
  creditLedgerSummary: HttpState<CreditLedgerSummaryDto>;
}
