namespace Desnz.Chmm.Common.Constants;
public static class BoilerSalesStatusConstants
{
    /// <summary>
    /// Status' for Boiler Sales data submission
    /// </summary>
    public static class BoilerSalesStatus
    {
        /// <summary>
        /// Boiler Sales data is not required yet
        /// </summary>
        public const string Default = "N/A";

        /// <summary>
        /// Boiler Sales data is now required
        /// </summary>
        public const string Due = "Due";

        /// <summary>
        /// Boiler sales data has been submitted by the manufacturer
        /// </summary>
        public const string Submitted = "Submitted";

        /// <summary>
        /// Unused in GT
        /// </summary>
        public const string AwaitingReview = "Awaiting review";

        /// <summary>
        /// Unused in GT
        /// </summary>
        public const string InReview = "In review";

        /// <summary>
        /// Unused in GT
        /// </summary>
        public const string WaitingForInformation = "Waiting for information";

        /// <summary>
        /// Unused in GT
        /// </summary>
        public const string WaitingForPeerReview = "Waiting for peer review";

        /// <summary>
        /// Unused in GT
        /// </summary>
        public const string InPeerReview = "In peer review";

        /// <summary>
        /// Boiler Sales data has now been approved by the admin
        /// </summary>
        public const string Approved = "Approved";
    }

    public static class BoilerSalesSummarySubmissionStatus
    {
        public const string NotSubmitted = "Not Submitted";
        public const string QuarterSubmitted = "Quarter Submitted";
        public const string AnnualSubmitted = "Annual Submitted";
        public const string AnnualApproved = "Annual Approved";
    }
}