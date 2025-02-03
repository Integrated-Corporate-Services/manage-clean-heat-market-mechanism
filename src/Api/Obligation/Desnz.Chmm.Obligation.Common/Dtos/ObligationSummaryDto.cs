
namespace Desnz.Chmm.Obligation.Common.Dtos
{
    /// <summary>
    /// Details of the obligation for a given year
    /// </summary>
    public class ObligationSummaryDto
    {
        public ObligationSummaryDto(
            int generatedObligations,
            int obligationsBroughtForward,
            List<ObligationSummaryItemDto> obligationAmendments,
            int finalObligations,
            int obligationsCarriedOver,
            int obligationsPaidOff,
            int remainingObligations
        )
        {
            GeneratedObligations = generatedObligations;
            ObligationsBroughtForward = obligationsBroughtForward;
            ObligationAmendments = obligationAmendments;
            FinalObligations = finalObligations;
            ObligationsCarriedOver = obligationsCarriedOver;
            ObligationsPaidOff = obligationsPaidOff;
            RemainingObligations = remainingObligations;
        }

        public int GeneratedObligations { get; private set; }
        public int ObligationsBroughtForward { get; private set; }
        public List<ObligationSummaryItemDto> ObligationAmendments { get; private set; }
        public int FinalObligations { get; private set; }
        public int ObligationsCarriedOver { get; private set; }
        public int ObligationsPaidOff { get; private set; }
        public int RemainingObligations { get; private set; }
    }
}
