namespace Desnz.Chmm.CommonValidation
{
    public class OrganisationValidation
    {
        public Guid OrganisationId { get; init; }
        
        [System.ComponentModel.DefaultValue(false)]
        public bool RequireActiveOrganisation { get; init; }
    }
}
