using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Commands.Annual;

public class SubmitAnnualBoilerSalesCommand : IRequest<ActionResult<Guid>>
{
    public Guid OrganisationId { get; set; }
    public Guid SchemeYearId { get; set; }
    public int Oil { get; set; }
    public int Gas { get; set; }
}
