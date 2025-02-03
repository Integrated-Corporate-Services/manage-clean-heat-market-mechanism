namespace Desnz.Chmm.Obligation.Common.Dtos
{
    public class CreditRedemptionDto
    {
        public CreditRedemptionDto(Guid organisationId, decimal value)
        {
            OrganisationId = organisationId;
            Value = Decimal.ToInt32(Math.Round(value, MidpointRounding.AwayFromZero)); ;
        }

        public Guid OrganisationId { get; }
        public int Value { get; }
    }
}
