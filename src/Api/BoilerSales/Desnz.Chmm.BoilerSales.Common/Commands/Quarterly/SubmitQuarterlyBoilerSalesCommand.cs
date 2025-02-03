using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Commands.Quarterly;

public class SubmitQuarterlyBoilerSalesCommand : BoilerSalesBase, IRequest<ActionResult<Guid>>
{
    public Guid OrganisationId { get; set; }
    public Guid SchemeYearId { get; set; }
    public Guid SchemeYearQuarterId { get; set; }
}
