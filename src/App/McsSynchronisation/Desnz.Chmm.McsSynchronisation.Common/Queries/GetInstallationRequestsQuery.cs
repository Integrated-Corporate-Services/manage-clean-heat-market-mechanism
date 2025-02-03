using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.McsSynchronisation.Common.Queries;

public class GetInstallationRequestsQuery : IRequest<ActionResult<List<InstallationRequestSummaryDto>>>
{

    /// <summary>
    /// Scheme year being queried
    /// </summary>
    public Guid SchemeYearId { get; }

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="schemeYearId"></param>
    public GetInstallationRequestsQuery(Guid schemeYearId)
    {
        SchemeYearId = schemeYearId;
    }
}
