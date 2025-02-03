namespace Desnz.Chmm.Identity.Api.Constants
{
    public static class OrganisationAddressConstants
    {
        public static class Type
        {
            public const string OfficeAddress = "Office Address";
            public const string LegalCorrespondenceAddress = "Legal Correspondence Address";
        }

        public static class LegalCorrespondenceAddressType
        {
            public const string UseRegisteredOffice = "Use Registered Office";
            public const string UseSpecifiedAddress = "Use Specified Address";
            public const string NoLegalCorrespondenceAddress = "No Legal Correspondence Address";

            public static string[] AllTypes = new[]
            {
                UseRegisteredOffice,UseSpecifiedAddress, NoLegalCorrespondenceAddress
            };
        }
    }
}
