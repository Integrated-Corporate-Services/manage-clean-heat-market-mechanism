export interface UpdateSchemeYearConfigurationCommand {
  schemeYearId: string;
  percentageCap: number;
  targetMultiplier: number;
  gasBoilerSalesThreshold: number;
  oilBoilerSalesThreshold: number;
  targetRate: number;
  creditCarryOverPercentage: number;
  alternativeRenewableSystemFuelTypeWeightingValue: number;
  alternativeFossilFuelSystemFuelTypeWeightingValue: number;
}
