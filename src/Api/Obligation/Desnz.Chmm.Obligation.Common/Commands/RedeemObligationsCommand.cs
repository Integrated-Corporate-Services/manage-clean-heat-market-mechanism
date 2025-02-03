using Desnz.Chmm.Obligation.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Obligation.Common.Commands;

public class RedeemObligationsCommand : IRequest<ActionResult>
{
    public RedeemObligationsCommand(Guid schemeYearId, List<CreditRedemptionDto> redemptions)
    {
        SchemeYearId = schemeYearId;
        Redemptions = redemptions;
    }

    public Guid SchemeYearId { get; }
    public List<CreditRedemptionDto> Redemptions { get; }
}
