export type BoilerSalesSubmissionStatus =
  | 'Not Submitted'
  | 'Quarter Submitted'
  | 'Annual Submitted'
  | 'Annual Approved';

export interface BoilerSalesSummaryDto {
  oilBoilerSales: number;
  gasBoilerSales: number;
  oilBoilerSalesAboveThreshold: number;
  gasBoilerSalesAboveThreshold: number;
  sumOfBoilerSales: number;
  sumOfBoilerSalesAboveThreshold: number;
  boilerSalesSubmissionStatus: BoilerSalesSubmissionStatus;
}
