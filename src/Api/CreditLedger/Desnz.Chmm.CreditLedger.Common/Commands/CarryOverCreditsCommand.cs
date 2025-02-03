using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Common.Commands;

public class CarryOverCreditsCommand : IRequest<ActionResult>
{
    public Guid SchemeYearId { get; private set; }

    public CarryOverCreditsCommand(Guid schemeYearId) => SchemeYearId = schemeYearId;
}
