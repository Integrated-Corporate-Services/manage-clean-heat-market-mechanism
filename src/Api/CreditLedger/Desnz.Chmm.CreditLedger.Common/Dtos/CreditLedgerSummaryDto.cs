namespace Desnz.Chmm.CreditLedger.Common.Dtos
{
    public class CreditLedgerSummaryDto
    {
        public CreditLedgerSummaryDto(
            decimal creditsGeneratedByHeatPumps,
            decimal creditsGeneratedByHybridHeatPumps,
            decimal creditsTransferred,
            decimal creditsBoughtForward,
            IList<TransactionSummaryAdminAdjustmentDto> creditAmendments,
            decimal creditBalance,
            decimal creditsRedeemed,
            decimal creditsCarriedForward,
            decimal creditsExpired
            )
        {
            CreditsGeneratedByHeatPumps = creditsGeneratedByHeatPumps;
            CreditsGeneratedByHybridHeatPumps = creditsGeneratedByHybridHeatPumps;
            CreditsTransferred = creditsTransferred;
            CreditsBoughtForward = creditsBoughtForward;
            CreditAmendments = creditAmendments;
            CreditBalance = creditBalance;
            CreditsRedeemed = creditsRedeemed;
            CreditsCarriedForward = creditsCarriedForward;
            CreditsExpired = creditsExpired;
        }

        public decimal CreditsGeneratedByHeatPumps { get; private set; }
        public decimal CreditsGeneratedByHybridHeatPumps { get; private set; }
        public decimal CreditsTransferred { get; private set; }
        public decimal CreditsBoughtForward { get; private set; }
        public IList<TransactionSummaryAdminAdjustmentDto> CreditAmendments { get; private set; }
        public decimal CreditBalance { get; private set; }
        public decimal CreditsRedeemed { get; private set; }
        public decimal CreditsCarriedForward { get; private set; }
        public decimal CreditsExpired { get; private set; }
    }
}
