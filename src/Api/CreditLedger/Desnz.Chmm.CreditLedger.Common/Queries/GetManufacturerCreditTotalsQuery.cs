using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Common.Queries
{
    public class GetManufacturerCreditTotalsQuery : OrganisationSchemeYearQueryBase, IRequest<ActionResult<List<PeriodCreditTotals>>>
    {
        public GetManufacturerCreditTotalsQuery(Guid organisationId, Guid schemeYearId) : base(organisationId, schemeYearId)
        {
        }
    }
}
