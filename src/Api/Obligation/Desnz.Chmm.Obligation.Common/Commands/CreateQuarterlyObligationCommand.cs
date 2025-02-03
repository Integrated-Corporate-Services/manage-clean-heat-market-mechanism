namespace Desnz.Chmm.Obligation.Common.Commands
{
    public class CreateQuarterlyObligationCommand : CreateAnnualObligationCommand
    {
        public Guid SchemeYearQuarterId { get; set; }
    }
}
