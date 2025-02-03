export interface SchemeYearSummaryConfigurationDto {
  currentSchemeYear: string;
  previousSchemeYear: string;
  obligationCalculationPercentage: number;
  isAfterEndOfSchemeYearDate: boolean;
  isAfterSurrenderDay: boolean;
  isAfterPreviousSchemeYearSurrenderDate: boolean;
  isWithinAmendCreditsWindow: boolean;
  isWithinAmendObligationsWindow: boolean;
  isWithinCreditTranferWindow: boolean;
  alternativeRenewableSystemFuelTypeWeightingValue: number;
  alternativeFossilFuelSystemFuelTypeWeightingValue: number;
}
