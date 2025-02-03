using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.McsSynchronisation.Common.Queries;

public class GetAllInstallationRequestsQuery : IRequest<ActionResult<List<InstallationRequestSummaryDto>>>
{
}
