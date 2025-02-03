namespace Desnz.Chmm.Identity.Common.Dtos.Organisation
{
    public class ViewOrganisationDto
    {
        public bool IsNonSchemeParticipant { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public List<ViewOrganisationLicenceHolderDto> LicenceHolders { get; set; }
    }

    public class ViewOrganisationLicenceHolderDto
    {
        public string Name { get; set; }
    }
}
