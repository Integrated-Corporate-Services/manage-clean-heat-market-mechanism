using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Commands.Annual;

public class EditAnnualBoilerSalesCommand : BoilerSalesBase, IRequest<ActionResult>
{
    public EditAnnualBoilerSalesCommand(Guid organisationId, Guid schemeYearId, int gas, int oil)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
        Gas = gas;
        Oil = oil;
    }

    public Guid OrganisationId { get; set; }
    public Guid SchemeYearId { get; set; }
}
