using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Commands.Quarterly;

public class EditQuarterlyBoilerSalesCommand : BoilerSalesBase, IRequest<ActionResult>
{
    public EditQuarterlyBoilerSalesCommand(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId, int gas, int oil)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
        SchemeYearQuarterId = schemeYearQuarterId;
        Gas = gas;
        Oil = oil;
    }

    public Guid OrganisationId { get; set; }
    public Guid SchemeYearId { get; set; }
    public Guid SchemeYearQuarterId { get; set; }
}
