using Desnz.Chmm.CreditLedger.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Common.Queries;

/// <summary>
/// Query the credit balance for an organisation
/// </summary>
public class GetAllCreditBalancesQuery : IRequest<ActionResult<List<OrganisationCreditBalanceDto>>>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="schemeYearId">The Id of the scheme year to query</param>
    public GetAllCreditBalancesQuery(Guid schemeYearId)
    {
        SchemeYearId = schemeYearId;
    }

    /// <summary>
    /// Scheme year being queried
    /// </summary>
    public Guid SchemeYearId { get; }
}
