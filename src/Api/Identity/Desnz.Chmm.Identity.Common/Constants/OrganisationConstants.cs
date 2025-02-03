

namespace Desnz.Chmm.Identity.Common.Constants
{
    public static class OrganisationConstants
    {
        public static class Status
        {
            public const string Pending = "Pending";
            public const string Active = "Active";
            public const string Retired = "Retired";
            public const string Archived = "Archived";
        }
        public static class Buckets
        {
            public const string IdentityOrganisationApprovals = "identity-organisation-approvals";
            public const string IdentityOrganisationRejections = "identity-organisation-rejections";
            public const string IdentityOrganisationStructures = "identity-organisation-structures";
        }
    }
}
