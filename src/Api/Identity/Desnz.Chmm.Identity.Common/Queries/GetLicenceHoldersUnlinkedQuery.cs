using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

/// <summary>
/// Get all of the unlinked licence holders
/// </summary>
public class GetLicenceHoldersUnlinkedQuery : IRequest<ActionResult<List<LicenceHolderDto>>>
{
}
