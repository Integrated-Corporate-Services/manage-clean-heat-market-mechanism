
namespace Desnz.Chmm.Identity.Common.Dtos.Organisation
{
    public class OrganisationDto
    {
        public bool IsOnBehalfOfGroup { get; set; }
        public bool IsNonSchemeParticipant { get; set; }
        public string LegalAddressType { get; set; }
        public ResponsibleUndertakingDto ResponsibleUndertaking { get; set; }
        public bool IsFossilFuelBoilerSeller { get; set; }
        public string[]? HeatPumpBrands { get; set; }
        public CreditContactDetailsDto CreditContactDetails { get; set; }
    }
}
