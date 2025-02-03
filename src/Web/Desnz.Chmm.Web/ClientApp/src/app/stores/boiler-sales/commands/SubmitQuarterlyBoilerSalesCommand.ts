export interface SubmitQuarterlyBoilerSalesCommand {
  organisationId: string;
  schemeYearId: string;
  schemeYearQuarterId: string;
  gas: number;
  oil: number;
}
