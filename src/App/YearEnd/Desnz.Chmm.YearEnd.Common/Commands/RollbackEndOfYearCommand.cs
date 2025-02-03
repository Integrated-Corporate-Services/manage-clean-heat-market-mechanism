using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.YearEnd.Api.Controllers;

public class RollbackEndOfYearCommand : IRequest<ActionResult>
{

        public RollbackEndOfYearCommand(Guid schemeYearId)
        {
            SchemeYearId = schemeYearId;
        }

    public Guid SchemeYearId { get; private set; }
}