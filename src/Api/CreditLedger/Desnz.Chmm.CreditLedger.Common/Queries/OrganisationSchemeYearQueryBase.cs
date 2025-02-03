namespace Desnz.Chmm.CreditLedger.Common.Queries
{
    public class OrganisationSchemeYearQueryBase 
    {
        public OrganisationSchemeYearQueryBase(Guid organisationId, Guid schemeYearId)
        {
            OrganisationId = organisationId;
            SchemeYearId = schemeYearId;
        }

        public Guid OrganisationId { get; }
        public Guid SchemeYearId { get; }
    }
}
