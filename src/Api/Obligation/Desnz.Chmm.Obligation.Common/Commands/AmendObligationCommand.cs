using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Obligation.Common.Commands
{
    public class AmendObligationCommand : IRequest<ActionResult>
    {
        public Guid OrganisationId { get; set; }
        public Guid SchemeYearId { get; set; }
        public int Value { get; set; }
    }
}
