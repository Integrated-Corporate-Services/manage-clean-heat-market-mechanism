export interface TransactionSummaryAdminAdjustmentDto {
  dateOfTransaction: string;
  credits: number;
}

export interface CreditLedgerSummaryDto {
  creditsGeneratedByHeatPumps: number;
  creditsGeneratedByHybridHeatPumps: number;
  creditsTransferred: number;
  creditsBoughtForward: number;
  creditAmendments: TransactionSummaryAdminAdjustmentDto[];
  creditBalance: number;
  creditsRedeemed: number;
  creditsCarriedForward: number;
  creditsExpired: number;
}
