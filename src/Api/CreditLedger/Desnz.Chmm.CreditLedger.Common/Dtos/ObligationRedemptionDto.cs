namespace Desnz.Chmm.CreditLedger.Common.Dtos
{
    public class ObligationRedemptionDto
    {
        public ObligationRedemptionDto(Guid organisationId, decimal value)
        {
            OrganisationId = organisationId;            
            Value = Decimal.ToInt32(Math.Round(value, MidpointRounding.AwayFromZero));
        }

        public Guid OrganisationId { get; }
        public int Value { get; }
    }
}
