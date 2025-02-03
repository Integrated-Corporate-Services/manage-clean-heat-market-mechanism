using Desnz.Chmm.CreditLedger.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Common.Queries
{
    public class GetCreditLedgerTransfersQuery : OrganisationSchemeYearQueryBase, IRequest<ActionResult<CreditLedgerTransfersDto>>
    {
        public GetCreditLedgerTransfersQuery(Guid organisationId, Guid schemeYearId) : base(organisationId, schemeYearId)
        {
        }
    }
}
