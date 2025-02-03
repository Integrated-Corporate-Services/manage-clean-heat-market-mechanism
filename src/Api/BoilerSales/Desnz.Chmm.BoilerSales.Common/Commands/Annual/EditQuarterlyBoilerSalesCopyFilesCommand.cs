using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Commands.Quarterly;

public class EditQuarterlyBoilerSalesCopyFilesCommand : IRequest<ActionResult>
{
    public Guid OrganisationId { get; set; }
    public Guid SchemeYearId { get; set; }
    public Guid SchemeYearQuarterId { get; set; }
}
