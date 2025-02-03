using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Obligation.Common.Commands;

public class RollbackRedeemObligationsCommand : IRequest<ActionResult>
{
    public RollbackRedeemObligationsCommand(Guid schemeYearId)
    {
        SchemeYearId = schemeYearId;
    }

    public Guid SchemeYearId { get; }
}