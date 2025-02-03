using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Obligation.Common.Commands
{
    public class CreateAnnualObligationCommand : SalesNumbersCommand, IRequest<ActionResult>
    {
        public Guid OrganisationId { get; set; }
        public Guid SchemeYearId { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool Override { get; set; } = false;
    }
}
