namespace Desnz.Chmm.Identity.Common.Dtos
{
    public class ManufacturerOnboardingDto
    {
        public bool IsGroupRegistration { get; set; }
        public bool IsFossilFuelBoilerSeller { get; set; }
        public bool IsNonSchemeParticipant { get; set; }
        public string? Name { get; set; }
        public string? CompaniesHouseNumber { get; set; }
        public string? ContactName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactTelephoneNumber { get; set; }

        public OrganisationAddressDto? OfficeAddress { get; set; }
        public OrganisationAddressDto? LegalCorrespondenceAddress { get; set; }
        public ManufacturerUserDto? Applicant { get; set; }
        public ManufacturerUserDto? ResponsibleOfficer { get; set; }
        public string[]? HeatPumpBrands { get; set; }
    }
}
