namespace Desnz.Chmm.CreditLedger.Common.Dtos
{
    public class OrganisationLicenceHolderCreditsDto
    {
        public OrganisationLicenceHolderCreditsDto(Guid licenceHolderId, Guid organisationId, decimal credits)
        {
            LicenceHolderId = licenceHolderId;
            OrganisationId = organisationId;
            Credits = credits;
        }

        public Guid LicenceHolderId { get; private set; }
        public Guid OrganisationId { get; private set; }
        public decimal Credits { get; private set; }
    }
}
