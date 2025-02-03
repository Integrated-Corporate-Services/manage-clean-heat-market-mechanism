using Desnz.Chmm.CreditLedger.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Common.Queries;

/// <summary>
/// Query the credit balance for an organisation
/// </summary>
public class CreditBalanceQuery : IRequest<ActionResult<CreditBalanceDto>>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="organisationId">The Id of the orgnisation to query</param>
    /// <param name="schemeYearId">The Id of the scheme year to query</param>
    public CreditBalanceQuery(Guid organisationId, Guid schemeYearId)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
    }

    /// <summary>
    /// Organisation being queried
    /// </summary>
    public Guid OrganisationId { get; }

    /// <summary>
    /// Scheme year being queried
    /// </summary>
    public Guid SchemeYearId { get; }
}