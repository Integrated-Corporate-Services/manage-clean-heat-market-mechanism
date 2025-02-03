using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Obligation.Common.Commands;

public class RollbackCarryForwardObligationCommand : IRequest<ActionResult>
{
    public RollbackCarryForwardObligationCommand(Guid schemeYearId)
    {
        SchemeYearId = schemeYearId;
    }    

    public Guid SchemeYearId { get; }
}
