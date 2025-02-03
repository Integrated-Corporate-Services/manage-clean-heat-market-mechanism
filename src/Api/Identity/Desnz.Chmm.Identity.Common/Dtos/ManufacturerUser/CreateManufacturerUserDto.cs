namespace Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser
{
    public class CreateManufacturerUserDto
    {
        public string TelephoneNumber { get; set; }
        public string? ResponsibleOfficerOrganisationName { get; set; }
        public bool IsResponsibleOfficer { get; set; }
        public string Name { get; set; }
        public required string Email { get; set; }
        public string JobTitle { get; set; }
    }
}
