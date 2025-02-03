using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desnz.Chmm.YearEnd.Common.Commands;
public class RollbackRedemptionCommand : IRequest<ActionResult>
{
    public RollbackRedemptionCommand(Guid schemeYearId)
    {
        SchemeYearId = schemeYearId;
    }

    public Guid SchemeYearId { get; }
}
