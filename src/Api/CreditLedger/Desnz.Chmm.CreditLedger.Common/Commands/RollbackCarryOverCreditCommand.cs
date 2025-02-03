using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Common.Commands;

public class RollbackCarryOverCreditCommand : IRequest<ActionResult>
{
    public RollbackCarryOverCreditCommand(Guid schemeYearId)
    {
        SchemeYearId = schemeYearId;
    }

    public Guid SchemeYearId { get; }
}