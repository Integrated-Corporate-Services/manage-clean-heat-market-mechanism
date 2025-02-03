namespace Desnz.Chmm.Common.Constants
{
    public static class ConfigurationValueConstants
    {
        public const string BoilerSalesApiUrl = "BoilerSalesApiUrl";
        public const string CreditLedgerApiUrl = "CreditLedgerApiUrl";
        public const string IdentityApiUrl = "IdentityApiUrl";
        public const string ConfigurationApiUrl = "ConfigurationApiUrl";
        public const string ObligationApiUrl = "ObligationApiUrl";
        public const string YearEndApiUrl = "YearEndApiUrl";
        public const string McsSynchronisationApiUrl = "McsSynchronisationApiUrl";
        public const string McsMidApiUrl = "McsMidApiUrl";

        public const string OneLoginAuth = "OneLoginAuth";
        public const string ChmmJwtAuth = "ChmmJwtAuth";
        public const string GovukNotify = "GovukNotify";
        public const string ApiKeyPolicy = "ApiKeyPolicy";
        public const string EnvironmentConfig = "EnvironmentConfig";
        public const string ClamAvConfig = "ClamAv";
        public const string Proxy = "Proxy";
        public const string McsApi = "McsApi";
        public const string GoogleAnalytics = "GoogleAnalytics";

        public static string GetErrorMessage(string key) => $"Unable to get {key} from the application settings. Missing configuration value for key: {key}";
    }
}
