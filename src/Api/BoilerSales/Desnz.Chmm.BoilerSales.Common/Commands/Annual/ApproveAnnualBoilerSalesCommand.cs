using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Commands.Annual
{
    public class ApproveAnnualBoilerSalesCommand : IRequest<ActionResult>
    {
        public Guid OrganisationId { get; set; }
        public Guid SchemeYearId { get; set; }
    }
}
