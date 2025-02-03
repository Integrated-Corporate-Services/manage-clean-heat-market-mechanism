using Desnz.Chmm.CreditLedger.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Common.Queries
{
    /// <summary>
    /// Query the organisation's credit ledger summary for the given year
    /// </summary>
    public class GetCreditLedgerSummaryQuery : OrganisationSchemeYearQueryBase, IRequest<ActionResult<CreditLedgerSummaryDto>>
    {
        public GetCreditLedgerSummaryQuery(Guid organisationId, Guid schemeYearId) : base(organisationId, schemeYearId)
        {
        }
    }
}
