
namespace Desnz.Chmm.Obligation.Common.Dtos
{
    public class ObligationTotalDto
    {
        public ObligationTotalDto(Guid organisationId, int value)
        {
            OrganisationId = organisationId;
            Value = value;
        }

        public Guid OrganisationId { get; private set; }
        public int Value { get; private set; }
    }
}
