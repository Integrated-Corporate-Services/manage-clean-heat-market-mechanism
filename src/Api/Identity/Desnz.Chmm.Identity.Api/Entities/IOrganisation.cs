namespace Desnz.Chmm.Identity.Api.Entities
{
    public interface IOrganisation
    {
        void UpdateApplicant(string name, string jobTitle, string telephoneNumber);
        void UpdateCreditContactDetails(string? name, string? email, string? telephoneNumber);
        void UpdateDetails(string name, string? companiesHouseNumber);
        void UpdateFossilFuelSeller(bool isFossilFuelBoilerSeller);
        void UpdateHeatPumpDetails(string[]? heatPumpBrands);
        void RemoveLegalCorrespondenceAddressIfExists(string legalAddressType);
        OrganisationAddress? UpdateLegalCorrespondenceAddress(string legalAddressType, string lineOne, string? lineTwo, string city, string? county, string postcode);
        void UpdateOrganisationStructure(bool isOnBehalfOfGroup);
        void UpdateRegisteredOfficeAddress(string lineOne, string? lineTwo, string city, string? county, string postcode);
        void RemoveResponsibleOfficerIfExists();
        void UpdateSeniorResponsibleOfficerIfExists(string name, string jobTitle, string telephoneNumber);
        void UpdateSchemeParticipation(bool isNonSchemeParticipant);
        void UpdateSeniorResponsibleOfficerAssigned(Guid userId);
    }
}