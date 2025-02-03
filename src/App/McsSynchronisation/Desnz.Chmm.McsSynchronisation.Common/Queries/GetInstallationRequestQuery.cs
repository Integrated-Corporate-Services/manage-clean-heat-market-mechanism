using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.McsSynchronisation.Common.Queries;

public class GetInstallationRequestQuery : IRequest<ActionResult<DataPage<CreditCalculationDto>>>
{
    public GetInstallationRequestQuery(Guid installationRequestId, int pageNumber)
    {
        InstallationRequestId = installationRequestId;
        PageNumber = pageNumber;
    }

    public Guid InstallationRequestId { get; private set; }
    public int PageNumber { get; }
}
