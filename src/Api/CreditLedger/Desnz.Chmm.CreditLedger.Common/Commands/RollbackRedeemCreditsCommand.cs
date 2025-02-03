using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Common.Commands;

public class RollbackRedeemCreditsCommand : IRequest<ActionResult>
{
    public RollbackRedeemCreditsCommand(Guid schemeYearId)
    {
        SchemeYearId = schemeYearId;
    }

    public Guid SchemeYearId { get; }
}