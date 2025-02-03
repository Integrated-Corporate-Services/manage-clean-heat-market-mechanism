using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

/// <summary>
/// Get all of the licence holders
/// </summary>
public class GetLicenceHoldersAllQuery : IRequest<ActionResult<List<LicenceHolderDto>>>
{
}
