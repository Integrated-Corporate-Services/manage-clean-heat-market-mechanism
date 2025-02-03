export interface ObligationSummaryItemDto {
  dateOfTransaction: string;
  value: number;
}

export interface ObligationSummaryDto {
  generatedObligations: number;
  obligationsBroughtForward: number;
  obligationAmendments: ObligationSummaryItemDto[];
  finalObligations: number;
  obligationsCarriedOver: number;
  obligationsPaidOff: number;
  remainingObligations: number;
}
