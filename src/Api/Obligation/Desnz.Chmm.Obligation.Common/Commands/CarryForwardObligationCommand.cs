using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Obligation.Common.Commands
{
    public class CarryForwardObligationCommand : IRequest<ActionResult>
    {
        public CarryForwardObligationCommand(Guid schemeYearId)
        {
            SchemeYearId = schemeYearId;
        }

        public Guid SchemeYearId { get; set; }
    }
}
