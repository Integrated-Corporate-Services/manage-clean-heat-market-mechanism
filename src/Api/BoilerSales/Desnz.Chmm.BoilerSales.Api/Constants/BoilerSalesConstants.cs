
namespace Desnz.Chmm.BoilerSales.Constants;

public static class BoilerSalesConstants
{
    /// <summary>
    /// Bucket names for different files required in Boiler Sales
    /// </summary>
    public static class Buckets
    {
        /// <summary>
        /// Bucket for Annual Verification Statements
        /// </summary>
        public const string AnnualVerificationStatement = "boilersales-annual-verification-statement";

        /// <summary>
        /// Bucket for Annual Supporting Evidence Files
        /// </summary>
        public const string AnnualSupportingEvidence = "boilersales-annual-supporting-evidence";

        /// <summary>
        /// Bucket for Quarterly Supporting Evidence Files
        /// </summary>
        public const string QuarterlySupportingEvidence = "boilersales-quarterly-supporting-evidence";
    }

    /// <summary>
    /// The different types of files to be uploaded for Boiler Sales data
    /// </summary>
    public static class FileType
    {
        /// <summary>
        /// Supporting Evidence
        /// </summary>
        public const string SupportingEvidence = "Supporting evidence";

        /// <summary>
        /// Verification Statements
        /// </summary>
        public const string VerificationStatement = "Verification statement";
    }
}
