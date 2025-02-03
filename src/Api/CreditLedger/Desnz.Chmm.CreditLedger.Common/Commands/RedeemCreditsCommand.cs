using Desnz.Chmm.CreditLedger.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Common.Commands;

public class RedeemCreditsCommand : IRequest<ActionResult>
{
    public RedeemCreditsCommand(Guid schemeYearId, List<ObligationRedemptionDto> redemptions)
    {
        SchemeYearId = schemeYearId;
        Redemptions = redemptions;
    }

    public Guid SchemeYearId { get; }
    public List<ObligationRedemptionDto> Redemptions { get; }
}
