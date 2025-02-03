namespace Desnz.Chmm.Common.Constants
{
    public static class IdentityConstants
    {
        public static class System
        {
            public static Guid SystemUserId { get { return new Guid("bac4744d-4eda-47be-ab41-9e95d322c52b"); } }
        }

        public static class Authentication
        {
            public const string TokenName = "chmm_token";
        }

        public static class Roles
        {
            public const string Manufacturer = "Manufacturer";
            public const string RegulatoryOfficer = "Regulatory Officer";
            public const string SeniorTechnicalOfficer = "Senior Technical Officer";
            public const string PrincipalTechnicalOfficer = "Principal Technical Officer";
            public const string ApiRole = "API Role";

            public const string Admins = $"{RegulatoryOfficer},{SeniorTechnicalOfficer},{PrincipalTechnicalOfficer}";
            public const string AdminsAndManufacturer = $"{Admins},{Manufacturer}";
            public const string AdminsAndApi = $"{Admins},{ApiRole}";
            public const string Everyone = $"{AdminsAndManufacturer},{ApiRole}";

            public static readonly IEnumerable<string> AdminsList = new List<string>() { RegulatoryOfficer, SeniorTechnicalOfficer, PrincipalTechnicalOfficer };
        }

        public static class Claims
        {
            public const string OrganisationId = "OrganisationId";
            public const string Status = "http://schemas.microsoft.com/ws/2008/06/identity/claims/status";
        }
    }
}
