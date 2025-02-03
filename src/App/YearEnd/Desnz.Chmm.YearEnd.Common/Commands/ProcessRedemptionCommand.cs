using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.YearEnd.Common.Commands;

public class ProcessRedemptionCommand : IRequest<ActionResult>
{
    public ProcessRedemptionCommand(Guid schemeYearId)
    {
        SchemeYearId = schemeYearId;
    }

    public Guid SchemeYearId { get; }
}
