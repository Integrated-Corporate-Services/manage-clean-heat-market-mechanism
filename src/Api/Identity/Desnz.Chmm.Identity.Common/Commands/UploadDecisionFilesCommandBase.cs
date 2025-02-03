using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands;

public abstract class UploadDecisionFilesCommandBase : IRequest<ActionResult>
{
    public UploadDecisionFilesCommandBase(Guid organisationId, List<IFormFile> files)
    {
        OrganisationId = organisationId;
        Files = files;
    }

    public Guid OrganisationId { get; private set; }
    public List<IFormFile> Files { get; private set; }
}
