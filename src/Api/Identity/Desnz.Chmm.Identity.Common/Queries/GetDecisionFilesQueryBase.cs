using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries;

public abstract class GetDecisionFilesQueryBase : IRequest<ActionResult<List<string>>>
{
    public GetDecisionFilesQueryBase(Guid organisationId)
    {
        OrganisationId = organisationId;
    }

    public Guid OrganisationId { get; private set; }
}
