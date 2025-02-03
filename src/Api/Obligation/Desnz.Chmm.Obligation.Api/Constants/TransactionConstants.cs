namespace Desnz.Chmm.Obligation.Api.Constants
{
    public static class TransactionConstants
    {
        public static class TransactionType
        {
            public const string BroughtFowardFromPreviousYear = "BroughtForward";
            public const string CarryForwardToNextYear = "CarryForward";
            public const string QuarterlySubmission = "Quarterly Submission";
            public const string AnnualSubmission = "Annual Submission";
            public const string AdminAdjustment = "Admin Adjustment";
            public const string Redeemed = "Redeemed";
        }

        public static readonly List<string> TransactionTypes = new() {
            TransactionType.BroughtFowardFromPreviousYear,
            TransactionType.QuarterlySubmission,
            TransactionType.AnnualSubmission,
            TransactionType.AdminAdjustment,
            TransactionType.CarryForwardToNextYear,
            TransactionType.Redeemed
        };
    }
}
