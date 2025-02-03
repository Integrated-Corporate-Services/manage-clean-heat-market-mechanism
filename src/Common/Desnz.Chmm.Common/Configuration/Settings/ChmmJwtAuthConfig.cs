namespace Desnz.Chmm.Common.Configuration.Settings
{
    public class ChmmJwtAuthConfig
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecurityKey { get; set; }
        public short MinutesUntilExpiration { get; set; }
        public short ClockSkewSeconds { get; set; }
    }
}